using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

/// <summary>
/// HttpBaseClient is a class for all http(s) requests in your project
/// 
/// Has:
/// - a retry handler configured through constructor
///     - if request fails and retry is True, the retry request uses a new tcp connection and gets a fixed 10 seconds timeout
///     - a retry request occurs only for GET, HEAD or OPTION request methods, never for PUT/POST/DELETE
/// 
/// - a timeout handler configurable through constructor, but also per method
/// 
/// - each underlying tcp connection is cached for up to 5 minutes
/// 
/// Use HttpBaseClient directly or inherit from it, see the examples
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
public partial class HttpBaseClient
{
    const int DefaultTimeoutMilliseconds = 60000;

    bool RetryOnceOnRequestCancelled;
    bool IgnoreSslErrors;
    int TimeoutMilliseconds;

    /// <summary>
    /// </summary>
    /// <param name="retryOnceOnRequestCancelled">Retry with a fixed 10 seconds timeout upon a request was cancelled</param>
    /// <param name="ignoreSslErrors">Set to true if you want to ignore errors such as Ssl Cert Expired</param>
    /// <param name="defaultTimeoutMilliseconds">Default is 60 seconds</param>
    public HttpBaseClient(
        bool retryOnceOnRequestCancelled = false,
        bool ignoreSslErrors = true,
        int defaultTimeoutMilliseconds = DefaultTimeoutMilliseconds)
    {
        RetryOnceOnRequestCancelled = retryOnceOnRequestCancelled;
        IgnoreSslErrors = ignoreSslErrors;
        TimeoutMilliseconds = defaultTimeoutMilliseconds;
    }

    /// <summary>
    /// Send a HTTP HEAD request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = new HttpBaseClient().Head&lt;string&gt;("https://www.systemlibrary.com/head", MediaType.json, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Head<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
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
    /// var response = await new HttpBaseClient().HeadAsync&lt;string&gt;("https://www.systemlibrary.com/head", MediaType.json, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> HeadAsync<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
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
    /// var response = new HttpBaseClient().Delete&lt;string&gt;("https://www.systemlibrary.com/delete", deleteId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Delete<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
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
    /// var response = await new HttpBaseClient().DeleteAsync&lt;string&gt;("https://www.systemlibrary.com/delete", deleteId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> DeleteAsync<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
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
    /// var response = new HttpBaseClient().Put&lt;string&gt;("https://www.systemlibrary.com/put", putId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Put<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
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
    /// var response = await new HttpBaseClient().PutAsync&lt;string&gt;("https://www.systemlibrary.com/put", putId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> PutAsync<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Put, url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP GET request
    /// </summary>
    /// <example>
    /// <code>
    /// class Client : HttpBaseClient 
    /// {
    ///     public string Get()
    ///     {
    ///         return base.Get&lt;string&gt;("https://www.systemlibrary.com/", MediaType.json, 2000).Data;    
    ///     }
    /// } 
    /// 
    /// var client = new Client();
    /// 
    /// var json = client.Get();
    /// //json the text from the response, if server sent json formatted text, that is what it now contains
    /// //if server sends XML response, the XML is not converted to json... the response from server is at is it in string format
    /// </code>
    /// </example>
    public ClientResponse<T> Get<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return GetAsync<T>(url, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP GET async request
    /// </summary>
    /// <example>
    /// <code>
    /// var getId = 1;
    /// var response = new await HttpBaseClient().GetAsync&lt;string&gt;("https://www.systemlibrary.com/get", getId, MediaType.json, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> GetAsync<T>(string url, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Get, url, null, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP POST request
    /// </summary>
    /// <example>
    /// <code>
    /// var postId = 1;
    /// var response = new HttpBaseClient().Post&lt;string&gt;("https://www.systemlibrary.com/post", postId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Post<T>(string url, object data = null, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
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
    /// var response = await new HttpBaseClient().PostAsync&lt;string&gt;("https://www.systemlibrary.com/post", postId, MediaType.textplain, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> PostAsync<T>(string url, object data, MediaType mediaType = MediaType.json, int timeoutMilliseconds = DefaultTimeoutMilliseconds, IDictionary<string, string> headers = null, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Post, url, data, mediaType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }
}