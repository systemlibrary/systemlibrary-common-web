using System.Net.Http;
using System.Net.Http.Headers;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        partial class Request
        {
            static bool IsRequestHeader(string header)
            {
                return true;
            }

            static HttpRequestMessage GetMessage(RequestOptions options)
            {
                var message = new HttpRequestMessage(options.Method, options.Url);

                message.Content = options.Content;

                if (options.Headers != null)
                    foreach (var header in options.Headers)
                        if(IsRequestHeader(header.Key))
                            message.Headers.TryAddWithoutValidation(header.Key, header.Value);
                
                if (options.MediaType != MediaType.None && 
                    options.MediaType != MediaType.multipartFormData && 
                    options.Headers != null &&
                    (options.Headers.Count == 0 ||
                    !options.Headers.ContainsKey("accept") &&
                    !options.Headers.ContainsKey("Accept") &&
                    !options.Headers.ContainsKey("ACCEPT")))
                    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(options.MediaType.ToValue()));

                return message;
            }
        }
    }
}