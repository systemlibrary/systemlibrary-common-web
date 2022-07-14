using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Web;

/// <summary>
/// An implementation of the 'old' thread safe singleton HttpContext we all know and love from .NET Framework
/// </summary>
public static class HttpContextInstance
{
    static IHttpContextAccessor HttpContextAccessor;

    /// <summary>
    /// Returns current Http Context
    /// </summary>
    public static HttpContext Current => HttpContextAccessor?.HttpContext;

    internal static void Initialize(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }
}
