using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    static int? _CustomTimeoutMilliseconds;
    static int CustomTimeoutMilliseconds
    {
        get
        {
            if(_CustomTimeoutMilliseconds == null)
            {
                if (AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.TimeoutMilliseconds != DefaultTimeoutMilliseconds && AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.TimeoutMilliseconds > 0)
                {
                    _CustomTimeoutMilliseconds = AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.TimeoutMilliseconds;
                }
                else
                {
                    _CustomTimeoutMilliseconds = 0;
                }
            }

            return _CustomTimeoutMilliseconds.Value;
        }
    }

    async Task<ClientResponse<T>> SendAsync<T>(HttpMethod method, string url, object data, MediaType mediaType, int timeoutMilliseconds, IDictionary<string, string> headers, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)
    {
        //Timeout is currently set to the default one, lets see if it shouldve been overridden by other settings
        if (timeoutMilliseconds == DefaultTimeoutMilliseconds)
        {
            //Is it overridden per HttpBaseClient?
            if (TimeoutMilliseconds != DefaultTimeoutMilliseconds)
                timeoutMilliseconds = TimeoutMilliseconds;

            //Is it overridden by having a valid appSettings value?
            else if (CustomTimeoutMilliseconds > 0)
                timeoutMilliseconds = CustomTimeoutMilliseconds;
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
            RetryOnTransientErrors = UseRetryOnErrorPolicy,
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
