using System.Net.Http;
using System.Text;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    partial class Content
    {
        static HttpContent GetBodyPlainText(object data, Encoding encoding = null, MediaType mediaType = MediaType.plain)
        {
            return new StringContent(data is string ? data as string : data.ToString(), encoding != null ? encoding : Encoding.UTF8, mediaType.ToValue());
        }
    }
}