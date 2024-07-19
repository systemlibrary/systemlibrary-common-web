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

        if (ex != null)
        {
            if (ThrowOnUnsuccessful)
                throw ex;

            var exception = new Exception("Url (" + options.Method + ") " + options.Url + " failed with retry policy: " + options.UseRetryPolicy, ex);
            Log.Error(exception);
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
