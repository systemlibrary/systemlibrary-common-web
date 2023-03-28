using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;

namespace SystemLibrary.Common.Web.Extensions;

public static class HttpRequestExtensions
{
    public static string Url(this HttpRequest request)
    {
        if (request == null) return null;

        return request.Scheme + "://" + request.Host + request.Path + request.QueryString.Value;
    }

    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Headers != null && request.Headers.ContainsKey("X-Requested-With"))
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";

        return false;
    }

    public static Uri Referer(this HttpRequest request)
    {
        if(request == null) throw new ArgumentNullException(nameof(request));

        RequestHeaders header = request.GetTypedHeaders();

        return header?.Referer;
    }
}

