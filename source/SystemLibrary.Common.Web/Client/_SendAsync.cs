using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Polly;

namespace SystemLibrary.Common.Web;

partial class Client
{
    static HttpRequestException GetHttpRequestException(RequestOptions options, HttpResponseMessage response = null)
    {
        var message = "";
        if (response != null)
        {
            message = GetResponseBodyAsync(response)
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult() ?? "";

            response.Dispose();

            var messageIndex = message.IndexOf("\"message\"");
            if (messageIndex >= 0)
                message = message.Substring(messageIndex);
        }

        return new HttpRequestException($"Url ({options.Method}) {options.Url} failed with retry policy: {options.UseRetryPolicy}. Response status: {response?.StatusCode}, {response?.ReasonPhrase} " + message);
    }

    async Task<(HttpResponseMessage, Exception)> RetrySendWithCircuitBreakerAsync(RequestOptions options)
    {
        var policy = CircuitBreaker.GetPolicy(options);

        HttpResponseMessage response = null;
        Exception ex = null;

        try
        {
            await policy.ExecuteAsync(async () =>
            {
                (response, ex) = await Request.RetrySendAsync(options).ConfigureAwait(false);

                if (ex != null) throw ex;

                if (response?.StatusCode == HttpStatusCode.TooManyRequests ||
                    response?.StatusCode == HttpStatusCode.BadGateway ||
                    response?.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    response?.StatusCode == HttpStatusCode.NotFound)
                {
                    throw GetHttpRequestException(options, response);
                }
                return response;
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            if (ThrowOnUnsuccessful) throw;

            ex = e;
        }

        return (response, ex);

    }

    async Task<ClientResponse<T>> SendAsync<T>(HttpMethod method, string url, object data, MediaType mediaType, int timeout, IDictionary<string, string> headers, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)
    {
        if (url.IsNot())
            throw new Exception("Url is missing when trying to make a " + method + " request");

        var options = GetRequestOptions(method, url, data, mediaType, timeout, headers, jsonSerializerOptions, cancellationToken);

        HttpResponseMessage response = null;
        Exception ex = null;

        if (UseCircuitBreakerPolicy)
            (response, ex) = await RetrySendWithCircuitBreakerAsync(options).ConfigureAwait(false);
        else
            (response, ex) = await Request.RetrySendAsync(options).ConfigureAwait(false);

        var isSuccess = !response?.IsSuccessStatusCode == true;

        if (ThrowOnUnsuccessful)
        {
            if (ex != null)
                throw ex;
            
            if(!isSuccess)
                throw GetHttpRequestException(options, response);
        }
        else if (!isSuccess)
            Warning(options, response, ex);

        var responseData = await ReadResponseAsync<T>(url, response, cancellationToken, jsonSerializerOptions).ConfigureAwait(false);

        return new ClientResponse<T>(response, responseData);
    }

    static void Warning(RequestOptions options, HttpResponseMessage response, Exception ex = null)
    {
        var message = $"Url ({options.Method}) {options.Url} failed with retry policy: {options.UseRetryPolicy}. Response status code {response.StatusCode}, {response.ReasonPhrase}";
        if (ex != null)
        {
            Log.Warning(new HttpRequestException(message, ex));
        }
        else
        {
            Log.Warning(message);
        }
    }
}

internal static class CircuitBreaker
{
    static ConcurrentDictionary<string, IAsyncPolicy> Policies = new ConcurrentDictionary<string, IAsyncPolicy>();

    static IAsyncPolicy CreatePolicy()
    {
        return Policy.Handle<HttpRequestException>()
            .CircuitBreakerAsync(15, TimeSpan.FromSeconds(7));
    }

    internal static IAsyncPolicy GetPolicy(Client.RequestOptions options)
    {
        var uri = new Uri(options.Url);

        // TODO: Consider vary by "content != null" and "media type"
        var key = $"{uri.Scheme}{uri.Authority}{uri.Port}{uri.AbsolutePath}#{options.Method}";

        return Policies.GetOrAdd(key, CreatePolicy());
    }
}