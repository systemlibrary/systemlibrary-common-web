using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace SystemLibrary.Common.Web;

partial class Client
{
    RequestOptions GetRequestOptions(HttpMethod method, string url, object data, MediaType mediaType, int timeout, IDictionary<string, string> headers, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)
    {
        var content = ClientHttpContent.Get(data, mediaType, jsonSerializerOptions);

        timeout = GetTimeout(timeout, TimeoutConfig, DefaultTimeout);

        return new RequestOptions()
        {
            Method = method,
            Url = url,
            Headers = headers,
            MediaType = mediaType,
            Content = content,

            ForceNewClient = false,

            CancellationToken = cancellationToken,

            UseRetryPolicy = UseRetryPolicy,
            IgnoreSslErrors = IgnoreSslErrors,

            Timeout = timeout,
            RetryTimeout = RetryTimeout,
        };
    }

    static int GetTimeout(int timeout, int timeoutConfig, int defaultTimeout)
    {
        // Timeout is default, no custom passed in
        if (timeout == defaultTimeout)
        {
            // Use the global config timeout from appSettings
            return timeoutConfig;
        }

        // Validate custom input timeout, else use config timeout
        if (timeout <= 0) return timeoutConfig;

        return timeout;
    }
}
