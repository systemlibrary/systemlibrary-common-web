using System.Net.Http;

using SystemLibrary.Common.Net.Extensions;

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

                if(options.Headers != null)
                {
                    foreach (var header in options.Headers)
                        if (header.Key.Is())
                            message.Headers.TryAddWithoutValidation(header.Key, header.Value);                    
                }

                message.Headers.TryAddWithoutValidation("Accept", "*/*");
                message.Headers.TryAddWithoutValidation("Connection", "Keep-Alive");

                if (options.MediaType == MediaType.plain)
                    message.Headers.TryAddWithoutValidation("Content-Type", MediaType.plain.ToValue());

                else if (options.MediaType == MediaType.json)
                    message.Headers.TryAddWithoutValidation("Content-Type", MediaType.json.ToValue());

                else if (options.MediaType == MediaType.xml)
                    message.Headers.TryAddWithoutValidation("Content-Type", MediaType.xml.ToValue());

                return message;
            }
        }
    }
}