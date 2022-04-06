using System.Net.Http;
using System.Text;
using System.Text.Json;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        partial class Content
        {
            internal static HttpContent GetContent(object data, MediaType mediaType, JsonSerializerOptions jsonSerializerOptions)
            {
                HttpContent content = null;
                if (data == null) return content;

                switch (mediaType)
                {
                    case MediaType.textplain:
                        content = GetBodyPlainText(data);
                        break;

                    case MediaType.json:
                        content = GetBodyJson(data, null, jsonSerializerOptions);
                        break;

                    case MediaType.xwwwformUrlEncoded:
                        content = GetBodyXwwwFormUrlEncoded(data);
                        break;

                    case MediaType.multipartFormData:
                        content = GetBodyMultipartFormData(data);
                        break;

                    case MediaType.octetStream:
                    case MediaType.html:
                    case MediaType.css:
                    case MediaType.javascript:
                    case MediaType.pdf:
                    case MediaType.zip:
                        throw new System.Exception("Not yet implemented: " + mediaType);

                    //TODO: Implement binary json formatter and mediaType application/bson
                    //MediaTypeFormatter bsonFormatter = new BsonMediaTypeFormatter();

                    default:
                        content = new StringContent(data is string ? data as string : data.ToString(), Encoding.UTF8);
                        break;
                }

                if (mediaType != MediaType.None)
                    if (!content.Headers.TryGetValues("Content-Type", out _))
                        content.Headers.TryAddWithoutValidation("Content-Type", mediaType.ToValue());

                return content;
            }
        }
    }
}
