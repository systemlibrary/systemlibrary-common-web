using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

/// <summary>
/// Client is a class for all http(s) requests in your project
/// 
/// Uses HttpClient and Polly behind the scenes for features such as:
/// - Reusable TCP connections
/// - Retries up to two times with a new TCP connection on transient errors
/// - Short cuircuit breaking per url
/// 
/// useRetryPolicy:
/// False: Request is eligible for retry if request is GET, POST or a file request and status code is 502 or 504
///  False: retries up to 1 time on GET and POST
/// True: same as 'false' but adds:
/// - retries up to 2 times if response is null
/// - retries once on 404 GET
/// - retries once on 500 GET, POST
/// - retries once on file requests
/// - retries once on OPTION, PATCH, HEAD, CONNECT, TRACE
/// - retries up to 2 times on 502, 504 on non file requests
/// - A retry policy built-in, and option to enable more retries policies through appsettings
/// - If a request file request or GET or POST fails with 502 or 504, it will be retried
/// - If useRetryPolicy is true, it will also retry 500 errors once, and 404 once
///     - if request fails and retry is True, the retry request uses a new tcp connection with 10 seconds timeout
///     - a retry request occurs only for GET, HEAD or OPTION request methods, never for PUT/POST/DELETE
/// 
/// - a timeout handler configurable through constructor, but also per method
/// 
/// - each underlying tcp connection is cached for up to 2 minutes
/// 
/// Use HttpBaseClient directly or inherit from it, see the examples
/// 
/// Configurations:
/// "systemLibraryCommonWeb": {
/// 	"client": {
/// 		"timeout": 60000,
/// 		"retryRequestTimeout": 10,
/// 		"cacheClientConnectionSeconds": 120
/// 	}
/// }
/// </summary>
/// <example>
/// A simple class to hold our Response
/// <code>
///class HttpBinResponse
///{
///    public string url { get; set; }
///}
///</code>
///
/// Our Client - you can new up HttpBaseClient directly if you like, but here we can reuse "apiUrl" (and other stuff, headers/what not) for all methods against the same client
///<code>
///class HttpBinClient : HttpBaseClient
///{
///    const string apiUrl = "http://httpbin.org";
///    
///    public HttpBinClient() : base(retryOnceOnRequestCancelled: true, defaultTimeoutMilliseconds: 5000, ignoreSslErrors: false)
///    {
///    }
///    
///    public HttpBinResponse Get()
///    {
///        return base.Get&lt;HttpBinResponse&gt;(apiUrl + "/get").Data;
///    }
///}
///</code>
///
/// Running the above Client and Response in a UnitTest project as such:
/// <code>
/// [TestMethod]
/// public void Test()
/// {
///     var client = new HttpBinClient();
/// 
///     var response = client.Get();
/// 
///     Assert.IsTrue(response.url.Contains("http"));
///     //Visit: http://httpbin.org/get to see the actual value of 'url', then you know this Assert statement is true
/// }
/// </code>
/// 
/// 
/// Another example of using the HttpBaseClient directly:
/// <code>
/// public void Test()
/// {
///     var client = new HttpBaseClient();
/// 
///     var response = client.Get&lt;string&gt;("http://httpbin.org/get");
/// 
///     Assert.IsTrue(response.Contains("http"));
///     //Response is now the whole json text that the url: http://httpbin.org/get is returning
/// }
/// </code>
/// </example>
public partial class Client
{
    bool UseRetryPolicy;
    bool IgnoreSslErrors;
    bool ThrowOnUnsuccessful;
    int Timeout;
    int RetryTimeout;
    bool UseRequestBreakerPolicy;

    public Client(
        bool? useRetryPolicy = null,
        bool? ignoreSslErrors = null,
        int? timeout = null,
        int? retryTimeout = null,
        bool? throwOnUnsuccessful = null,
        bool? useRequestBreakerPolicy = null)
    {
        UseRetryPolicy = useRetryPolicy ?? UseRetryPolicyConfig;
        IgnoreSslErrors = ignoreSslErrors ?? IgnoreSslErrorsConfig;
        ThrowOnUnsuccessful = throwOnUnsuccessful ?? ThrowOnUnsuccessfulConfig;
        UseRequestBreakerPolicy = useRequestBreakerPolicy ?? UseRequestBreakerPolicyConfig;

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