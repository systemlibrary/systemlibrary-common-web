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

        if (timeout != null)
            Timeout = timeout.Value;
        else
            Timeout = TimeoutConfig;

        if (retryTimeout != null)
            RetryTimeout = retryTimeout.Value;
        else
            RetryTimeout = RetryTimeoutConfig;
    }

    /// <summary>
    /// Send a HTTP GET request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = new Client().Get&lt;string&gt;("https://www.systemlibrary.com/get", MediaType.json, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Get<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return GetAsync<T>(url, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP GET request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = await new Client().GetAsync&lt;string&gt;("https://www.systemlibrary.com/get", MediaType.json, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> GetAsync<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Get, url, null, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP HEAD request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = new Client().Head&lt;string&gt;("https://www.systemlibrary.com/head", MediaType.json, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Head<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return HeadAsync<T>(url, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
          .ConfigureAwait(false)
          .GetAwaiter()
          .GetResult();
    }

    /// <summary>
    /// Send a HTTP HEAD async request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = await new Client().HeadAsync&lt;string&gt;("https://www.systemlibrary.com/head", MediaType.json, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> HeadAsync<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Head, url, null, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP DELETE request
    /// </summary>
    /// <example>
    /// <code>
    /// var deleteId = 1;
    /// var response = new Client().Delete&lt;string&gt;("https://www.systemlibrary.com/delete", deleteId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Delete<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return DeleteAsync<T>(url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
          .ConfigureAwait(false)
          .GetAwaiter()
          .GetResult();
    }

    /// <summary>
    /// Send a HTTP DELETE async request
    /// </summary>
    /// <example>
    /// <code>
    /// var deleteId = 1;
    /// var response = await new Client().DeleteAsync&lt;string&gt;("https://www.systemlibrary.com/delete", deleteId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> DeleteAsync<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Delete, url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP PUT request
    /// </summary>
    /// <example>
    /// <code>
    /// var putId = 1;
    /// var response = new Client().Put&lt;string&gt;("https://www.systemlibrary.com/put", putId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Put<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return PutAsync<T>(url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP PUT async request
    /// </summary>
    /// <example>
    /// <code>
    /// var putId = 1;
    /// var response = await new Client().PutAsync&lt;string&gt;("https://www.systemlibrary.com/put", putId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> PutAsync<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Put, url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP POST request
    /// </summary>
    /// <example>
    /// <code>
    /// var postId = 1;
    /// var response = new Client().Post&lt;string&gt;("https://www.systemlibrary.com/post", postId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Post<T>(string url, object data = null, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return PostAsync<T>(url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP POST async request
    /// </summary>
    /// <example>
    /// <code>
    /// var postId = 1;
    /// var response = await new Client().PostAsync&lt;string&gt;("https://www.systemlibrary.com/post", postId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> PostAsync<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Post, url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP OPTIONS request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = new Client().Options&lt;string&gt;("https://www.systemlibrary.com/options");
    /// </code>
    /// </example>
    public ClientResponse<T> Options<T>(string url, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
    {
        return OptionsAsync<T>(url, timeoutMilliseconds, headers, cancellationToken)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP OPTIONS request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = await new Client().OptionsAsyncT&lt;string&gt;("https://www.systemlibrary.com/options");
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> OptionsAsync<T>(string url, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Options, url, null, MediaType.None, timeoutMilliseconds, headers, default, cancellationToken)
            .ConfigureAwait(false);
    }

}