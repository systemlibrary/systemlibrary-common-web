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
            static HttpContent GetBodyJson(object data, Encoding encoding = null, JsonSerializerOptions jsonSerializerOptions = null, MediaType mediaType = MediaType.json)
            {
                if (data is string text) return new StringContent(text, encoding != null ? encoding : Encoding.UTF8, mediaType.ToValue());

                return new StringContent(data.ToJson(jsonSerializerOptions), encoding != null ? encoding : Encoding.UTF8, mediaType.ToValue());
            }
        }
    }
}