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
    static HttpRequestException GetHttpRequestException(RequestOptions options, HttpResponseMessage response = null, Exception ex = null)
    {
        string message = null;

        if (response != null)
            message = $" Response status: { response?.StatusCode}, { response?.ReasonPhrase}";

        return new HttpRequestException($"{options.Method} {options.Url} failed with type {options.MediaType} and retry policy {options.UseRetryPolicy}." + message, ex);
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

    bool IsEligibleForCircuitBreakerPolicy(RequestOptions options)
    {
        if (!UseCircuitBreakerPolicy) return false;

        return !options.Url.IsFile();
    }
    async Task<ClientResponse<T>> SendAsync<T>(HttpMethod method, string url, object data, MediaType mediaType, int timeout, IDictionary<string, string> headers, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)
    {
        if (url.IsNot())
            throw new Exception("Url is missing when trying to make a " + method + " request");

        var options = GetRequestOptions(method, url, data, mediaType, timeout, headers, jsonSerializerOptions, cancellationToken);

        HttpResponseMessage response = null;
        Exception ex = null;

        // Activated per client,
        var useCircuitBreakerPolicy = IsEligibleForCircuitBreakerPolicy(options);

        if (useCircuitBreakerPolicy)
            (response, ex) = await RetrySendWithCircuitBreakerAsync(options).ConfigureAwait(false);
        else
            (response, ex) = await Request.RetrySendAsync(options).ConfigureAwait(false);

        var isSuccess = response?.IsSuccessStatusCode == true;

        if (ThrowOnUnsuccessful)
        {
            if (ex != null || !isSuccess)
                throw GetHttpRequestException(options, response, ex);
        }
        else if (ex != null || !isSuccess)
            Log.Warning(GetHttpRequestException(options, response, ex));

        var responseData = await ReadResponseAsync<T>(url, response, cancellationToken, jsonSerializerOptions).ConfigureAwait(false);

        return new ClientResponse<T>(response, responseData);
    }
}

internal static class CircuitBreaker
{
    static ConcurrentDictionary<string, IAsyncPolicy> Policies = new ConcurrentDictionary<string, IAsyncPolicy>();

    static IAsyncPolicy CreatePolicy()
    {
        return Policy.Handle<HttpRequestException>()
            .CircuitBreakerAsync(1, TimeSpan.FromSeconds(7));
    }

    internal static IAsyncPolicy GetPolicy(Client.RequestOptions options)
    {
        var uri = new Uri(options.Url);

        // TODO: Consider vary by "content != null" & CurrentUser.IsAuth?
        var key = $"{uri.Scheme}{uri.Authority}{uri.Port}{uri.AbsolutePath.MaxLength(64)}{options.Method}{options.MediaType}" + HttpContextInstance.Current?.User?.Identity?.IsAuthenticated;

        return Policies.GetOrAdd(key, CreatePolicy());
    }
}