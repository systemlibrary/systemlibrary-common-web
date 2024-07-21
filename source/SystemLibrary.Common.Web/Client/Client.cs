using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

public partial class Client
{
    bool UseRetryPolicy;
    bool IgnoreSslErrors;
    bool ThrowOnUnsuccessful;
    int Timeout;
    int RetryTimeout;
    bool UseCircuitBreakerPolicy;

    public Client(
        bool? useRetryPolicy = null,
        bool? ignoreSslErrors = null,
        int? timeout = null,
        int? retryTimeout = null,
        bool? throwOnUnsuccessful = null,
        bool? useCircuitBreakerPolicy = null)
    {
        UseRetryPolicy = useRetryPolicy ?? UseRetryPolicyConfig;
        IgnoreSslErrors = ignoreSslErrors ?? IgnoreSslErrorsConfig;
        ThrowOnUnsuccessful = throwOnUnsuccessful ?? ThrowOnUnsuccessfulConfig;
        UseCircuitBreakerPolicy = useCircuitBreakerPolicy ?? UseCircuitBreakerPolicyConfig;

        Timeout = timeout ?? TimeoutConfig;
        RetryTimeout = retryTimeout ?? RetryTimeoutConfig;

        if(Timeout <= 0)
            Timeout = TimeoutConfig;

        if(RetryTimeout <= 0) 
            RetryTimeout = RetryTimeoutConfig;
    }

    public ClientResponse<T> Get<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return GetAsync<T>(url, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public async Task<ClientResponse<T>> GetAsync<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Get, url, null, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }
}