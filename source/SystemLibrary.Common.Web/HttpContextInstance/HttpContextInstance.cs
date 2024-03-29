using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Web;

/// <summary>
/// An implementation of the 'old' thread safe singleton HttpContext we all know and love from .NET Framework
/// </summary>
public static class HttpContextInstance
{
    internal static IHttpContextAccessor HttpContextAccessor;

    /// <summary>
    /// Returns current Http Context
    /// </summary>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// var httpContext = HttpContextInstance.Current;
    /// //Note: httpContext can be null, for instance in an Console Application or a Unit Test Application
    /// </code>
    /// </example>
    public static HttpContext Current => HttpContextAccessor?.HttpContext;
}
