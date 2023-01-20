using System.Net.Http;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        partial class Request
        {
            static HttpRequestMessage GetMessage(RequestOptions options)
            {
                var message = new HttpRequestMessage(options.Method, options.Url);

                message.Content = options.Content;

                return message;
            }
        }
    }
}