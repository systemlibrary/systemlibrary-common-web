using System.Net;
using System.Net.Http;

namespace SystemLibrary.Common.Web;

public partial class Client
{
    static void Throw(string url, HttpMethod method, HttpResponseMessage response = null, string msg = null)
    {
        if(response == null && msg == null)
        {
            throw new HttpRequestException("Error occured requesting response from (" + method + ") url: " + url);
        }

        if (msg != null)
            throw new System.Exception(msg);

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
            throw new HttpRequestException(HttpStatusCode.BadRequest + " (actual: " + (int)response.StatusCode + "): " + response.ReasonPhrase + " " + message + " type: (" + method + ") url: " + url);
        }

        throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase + ". " + message + " type: (" + method + ") url: " + url);
    }

   

}