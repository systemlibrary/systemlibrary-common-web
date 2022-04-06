using System.Net.Http;
using System.Text;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        partial class Content
        {
            static HttpContent GetBodyPlainText(object data, Encoding encoding = null)
            {
                return new StringContent(data is string ? data as string : data.ToString(), encoding != null ? encoding : Encoding.UTF8);
            }
        }
    }
}