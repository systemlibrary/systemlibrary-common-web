using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    async Task<ClientResponse<T>> SendAsync<T>(HttpMethod method, string url, object data, MediaType mediaType, int timeoutMilliseconds, IDictionary<string, string> headers, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)
    {
        //Timeout is currently set to the default one, lets see if it shouldve been overridden by other settings
        if (timeoutMilliseconds == DefaultTimeoutMilliseconds)
        {
            //Is it overridden per HttpBaseClient?
            if (TimeoutMilliseconds != DefaultTimeoutMilliseconds)
                timeoutMilliseconds = TimeoutMilliseconds;

            //Is it overridden by having a valid appSettings value?
            else if (AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.TimeoutMilliseconds != DefaultTimeoutMilliseconds && AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.TimeoutMilliseconds > 0)
                timeoutMilliseconds = AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.TimeoutMilliseconds;
            //TODO: This appSettings check should occur during construction of the client, so once per client, instead of over and over again...
        }

        if (url.IsNot())
            throw new System.Exception("Url is missing when trying to make a " + method + " request");

        var content = Content.GetContent(data, mediaType, jsonSerializerOptions);

        var requestOptions = new RequestOptions()
        {
            Method = method,
            Url = url,
            Headers = headers,
            MediaType = mediaType,
            Content = content,
            RetryOnceOnRequestCancelled = RetryOnceOnRequestCancelled,
            IgnoreSslErrors = IgnoreSslErrors,
            ForceNewClient = false,
            TimeoutMilliseconds = timeoutMilliseconds,
            CancellationToken = cancellationToken
        };

        var response = await Request.SendRequestAsync(requestOptions).ConfigureAwait(false);

        var responseData = await ReadResponseAsync<T>(url, response, cancellationToken, jsonSerializerOptions, ThrowOnUnsuccessfulStatusCode).ConfigureAwait(false);

        return new ClientResponse<T>(response, responseData);
    }
}
