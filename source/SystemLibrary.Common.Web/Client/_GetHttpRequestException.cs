using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web;

partial class Client
{
    static HttpRequestException GetHttpRequestException(RequestOptions options, HttpResponseMessage response = null, Exception ex = null)
    {
        var message = new StringBuilder("", 255);

        if(response != null)
            message.Append($"{(int)response?.StatusCode} ");

        message.Append($"{options.Method} {options.Url} as {options.MediaType.ToValue()} with timeout {options.Timeout}ms ");

        if(ex != null)
        {
            if(ex is TaskCanceledException tce)
            {
                if(tce.Message.Contains("task was"))
                    message.Append("has timed out or was canceled: " );
                else if(tce.Message.Contains("operation was"))
                    message.Append("operation was stopped, firewall or timeout: ");
            }
            else
            {
                message.Append("has invalid response: ");
            }
            message.Append(ex.Message);
        }
      
        if (response != null)
            message.Append($" Reason: {response?.ReasonPhrase}");
       
        return new HttpRequestException(message.ToString(), ex);
    }
}