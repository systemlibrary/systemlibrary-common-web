using System.Net;
using System.Net.Http;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        static void ThrowRequestException(HttpResponseMessage response)
        {
            var message = GetResponseBodyAsync(response)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult() ?? "";

            response.Dispose();

            var messageIndex = message.IndexOf("\"message\"");
            if (messageIndex >= 0)
                message = message.Substring(messageIndex);

            if ((int)response.StatusCode == 422)
            {
                throw new HttpRequestException(HttpStatusCode.BadRequest + " (actual: " + (int)response.StatusCode + "): " + response.ReasonPhrase + " " + message);
            }

            throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase + ". " + message);
        }
    }
}
