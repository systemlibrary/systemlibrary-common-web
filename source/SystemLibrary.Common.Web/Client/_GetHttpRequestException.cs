using System;
using System.Net.Http;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web;

partial class Client
{
    static HttpRequestException GetHttpRequestException(RequestOptions options, HttpResponseMessage response = null, Exception ex = null)
    {
        string suffix = null;

        string prefix = null;
        if (response != null)
        {
            suffix = $" Reason: {response?.ReasonPhrase}";
            prefix = $"{(int)response?.StatusCode} ";
        }

        return new HttpRequestException(prefix + $"{options.Method} {options.Url} getting valid response as media-type {options.MediaType.ToValue()} and retry policy {options.UseRetryPolicy}." + suffix, ex);
    }
}