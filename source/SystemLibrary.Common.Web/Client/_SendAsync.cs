using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class Client
{
    async Task<ClientResponse<T>> SendAsync<T>(HttpMethod method, string url, object data, MediaType mediaType, int timeout, IDictionary<string, string> headers, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)
    {
        if (url.IsNot())
            throw new Exception("Url is missing when trying to make a " + method + " request");

        var options = GetRequestOptions(method, url, data, mediaType, timeout, headers, jsonSerializerOptions, cancellationToken);

        HttpResponseMessage response = null;
        Exception ex = null;

        var useCircuitBreakerPolicy = IsEligibleForCircuitBreakerPolicy(options);

        if (useCircuitBreakerPolicy)
            (response, ex) = await RetrySendWithCircuitBreakerAsync(options).ConfigureAwait(false);
        else
            (response, ex) = await Request.RetrySendAsync(options).ConfigureAwait(false);

        var isSuccess = response?.IsSuccessStatusCode == true;

        if (ex != null || !isSuccess)
        {
            if (ThrowOnUnsuccessful)
                throw GetHttpRequestException(options, response, ex);
            else
                Log.Warning(GetHttpRequestException(options, response, ex));
        }

        var responseData = await ReadResponseAsync<T>(url, response, cancellationToken, jsonSerializerOptions).ConfigureAwait(false);

        return new ClientResponse<T>(response, responseData);
    }

    async Task<(HttpResponseMessage, Exception)> RetrySendWithCircuitBreakerAsync(RequestOptions options)
    {
        var policyKey = PolicyKeyConverter.Convert(options);

        // TODO: Enable a rate limiter towards all backend api's... set a limit of 2000 req/min? or so...
        // var rateLimiter = RateLimiter.GetPolicy(policyKey);

        var circuitBreaker = CircuitBreaker.GetPolicy(policyKey);

        HttpResponseMessage response = null;
        Exception ex = null;

        try
        {
            return await circuitBreaker.ExecuteAsync(async () =>
            {
                (response, ex) = await Request.RetrySendAsync(options).ConfigureAwait(false);

                // Check, create and throw an exception to trigger circuit breaker
                if (ex == null && (response == null ||
                    response?.StatusCode == HttpStatusCode.TooManyRequests ||
                    response?.StatusCode == HttpStatusCode.InternalServerError ||
                    response?.StatusCode == HttpStatusCode.BadGateway ||
                    response?.StatusCode == HttpStatusCode.GatewayTimeout ||
                    response?.StatusCode == HttpStatusCode.ServiceUnavailable))
                {
                    ex = GetHttpRequestException(options, response);
                    throw ex;
                }

                return (response, ex);
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Log.Warning("Circuit breaker threw exception", ex);

            return (response, ex ?? e);
        }
    }
}
