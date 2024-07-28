using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

/// <summary>
/// Client is a class for all http(s) requests in your project
/// <para>Uses HttpClient and Polly behind the scenes for features such as reusing tcp connections, retry on 502 and 504 status codes, and short request breaking on 20 exceptions in a row for 7 seconds</para>
/// Request is always eligible for retry if request is a file request or GET or POST and status code is 502 or 504
/// <para>Options:</para>
/// useRetryPolicy:
/// <list>
/// <item>True, adds:</item>
/// <item>- retries once on 404 GET</item>
/// <item>- retries once on 500 GET, POST</item>
/// <item>- retries once on OPTION, PATCH, HEAD, CONNECT, TRACE</item>
/// <item>- retries two times on GET 502, 504 errors</item>
/// <item>- retries once if response is null (no response yet/timeout...)</item>
/// </list>
/// </summary>
/// <remarks>
/// Each client can have its own timeout, which will generate a different HttpClient pool
/// Each method also take an additional timeout parameter, a different timeout will create a new HttpClient
/// Each underlying HttpClient is used for default 20 minutes, each TCP connection is reused for 4 minutes and 55 seconds
/// Every 502 and 504 response on GET, POST or file request will always be retries once, cannot be turned off
/// </remarks>
/// <example>
/// appSettings.json default configurations:
/// "client": {
///   "timeout": 40001,
///   "retryTimeout": 10000, // The second retry will use half this duration, which is only used if useRetryPolicy is true
///   "ignoreSslErrors": true,
///   "useRetryPolicy": true, // one retry on 502 and 504 is GET/POST, cannot be turned off
///   "throwOnUnsuccessful": false,
///   "useRequestBreakerPolicy": false,
///   "clientCacheDuration": 1200
/// }
/// You can simply "var client = new Client()" if you want and use the methods, or you can inherit it on your integrations for reusing headers, api url, timeout configuration, etc...
/// 
/// A simple class and HttpBinClient:
/// <code>
/// class HttpBinResponse
/// {
///     public string Url { get; set; }
/// }
///</code>
///<code>
/// class HttpBinClient : Client
/// {
///     const string apiUrl = "http://httpbin.org";
///     
///     public HttpBinClient() : base(useRetryPolicy: true)
///     {
///     }
///     
///     public HttpBinResponse Get()
///     {
///         return base.Get&lt;HttpBinResponse&gt;(apiUrl + "/get").Data;
///     }
/// }
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
///     Assert.IsTrue(response.Url.Contains("http"));
///     //Visit: http://httpbin.org/get to see the actual value of 'url', then you know this Assert statement is true
/// }
/// </code>
/// 
/// Another example of using the Client directly:
/// <code>
/// public void Test()
/// {
///     var client = new Client();
/// 
///     var response = client.Get&lt;string&gt;("http://httpbin.org/get");
/// 
///     Assert.IsTrue(response.Contains("http"));
///     //Response is now the whole json (or any data actually as a string) text that the url: http://httpbin.org/get is returning
/// }
/// </code>
/// Another example returning HttpResponseMessage as is, for you to read the stream/content of the response yourself:
/// <code>
/// public void Test() 
/// {
///     var client = new Client();
///     var response = = client.Get&lt;HttpResponseMessage&gt;("http://httpbin.org/get");
///     var httpResponseMessage = response.Data;
///     // httpResponseMessage now is ready to be read if you need to read it manually, as in: it's not a json/xml/serialization type of response, but maybe an Stream/Image you need to read...
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
    public ClientResponse<T> Get<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return GetAsync<T>(url, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public async Task<ClientResponse<T>> GetAsync<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Get, url, null, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public ClientResponse<T> Head<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return HeadAsync<T>(url, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public async Task<ClientResponse<T>> HeadAsync<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Head, url, null, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public ClientResponse<T> Delete<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return DeleteAsync<T>(url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public async Task<ClientResponse<T>> DeleteAsync<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Delete, url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public ClientResponse<T> Put<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return PutAsync<T>(url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public async Task<ClientResponse<T>> PutAsync<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Put, url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public ClientResponse<T> Post<T>(string url, object data = null, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string , T> deserialize = null)
    {
        return PostAsync<T>(url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public async Task<ClientResponse<T>> PostAsync<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Post, url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
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
    public ClientResponse<T> Options<T>(string url, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return OptionsAsync<T>(url, timeoutMilliseconds, headers, cancellationToken, deserialize)
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
    public async Task<ClientResponse<T>> OptionsAsync<T>(string url, int timeoutMilliseconds = DefaultTimeout, IDictionary<string, string> headers = null, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Options, url, null, MediaType.None, timeoutMilliseconds, headers, default, cancellationToken, deserialize)
            .ConfigureAwait(false);
    }
}