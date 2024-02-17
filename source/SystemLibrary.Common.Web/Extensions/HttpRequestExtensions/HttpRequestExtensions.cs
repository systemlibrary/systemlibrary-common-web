using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;

namespace SystemLibrary.Common.Web.Extensions;

public static class HttpRequestExtensions
{
    /// <summary>
    /// Returns full url of the request, includes protocol, path and query:
    /// </summary>
    /// <example>
    /// <code>
    /// var url = request.Url();
    /// // for example, url is now: https://www.systemlibrary.com/hello?world=1
    /// </code>
    /// </example>
    public static string Url(this HttpRequest request)
    {
        return request == null ? null : request.Scheme + "://" + request.Host + request.Path + request.QueryString.Value;
    }

    /// <summary>
    /// Returns true if the request is an ajax request represented by the header 'X-Requested-With'
    /// </summary>
    /// <example>
    /// <code>
    /// var isAjax = request.IsAjaxRequest();
    /// // true if header X-Requested-With was set to "XMLHttpRequest", else false
    /// </code>
    /// </example>
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        return request == null
            ? throw new ArgumentNullException(nameof(request))
            : request.Headers != null && request.Headers.ContainsKey("X-Requested-With")
&& request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    /// <summary>
    /// Returns the referer as Uri or null if not found
    /// </summary>
    /// <example>
    /// <code>
    /// var referer = request.IsAjaxRequest();
    /// // referer is now the referer from the Header request, or null if not existing
    /// </code>
    /// </example>
    public static Uri Referer(this HttpRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        RequestHeaders header = request.GetTypedHeaders();

        return header?.Referer;
    }
}

