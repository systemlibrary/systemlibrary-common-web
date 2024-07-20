using System;
using System.Collections.Generic;
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

        var content = ClientHttpContent.Get(data, mediaType, jsonSerializerOptions);

        var options = CreateOptions(method, url, content, mediaType, timeout, headers, cancellationToken);

        var (response, ex) = await Request.RetrySendAsync(options).ConfigureAwait(false);

        if(ThrowOnUnsuccessful)
        {
            if (ex != null)
                throw ex;

            if (!response.IsSuccessStatusCode)
            {
                var exception = new HttpRequestException("Url (" + options.Method + ") " + options.Url + " failed with retry policy: " + options.UseRetryPolicy + ". Response status " + response.StatusCode + ", " + response.ReasonPhrase);
                Log.Warning(exception);
            }
        }

        if (!response.IsSuccessStatusCode)
        {
            var exception = new HttpRequestException("Url (" + options.Method + ") " + options.Url + " failed with retry policy: " + options.UseRetryPolicy + ". Response status " + response.StatusCode + ", " + response.ReasonPhrase, ex);
            Log.Warning(exception);
        }
        else if (ex != null)
        {
            Log.Warning(ex);
        }
        
        if(ThrowOnUnsuccessful)
        {
            if(!response.IsSuccessStatusCode)
            {
                Throw(options.Url, options.Method, response);
            }
        }    

        var responseData = await ReadResponseAsync<T>(url, response, cancellationToken, jsonSerializerOptions).ConfigureAwait(false);

        return new ClientResponse<T>(response, responseData);
    }
}
