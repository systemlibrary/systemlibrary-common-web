using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Web.Extensions;

public static class HttpRequestExtensions
{
    public static string Url(this HttpRequest request)
    {
        if (request == null) return null;

        return request.Scheme + "://" + request.Host + request.Path + request.QueryString.Value;
    }
}

