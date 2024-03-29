using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SystemLibrary.Common.Web;

/// <summary>
/// An implementation of the 'old' thread safe singleton ActionContext we all know and love from .NET Framework
/// </summary>
public static class ActionContextInstance
{
    internal static IActionContextAccessor ActionContextAccessor;

    /// <summary>
    /// Returns current Action Context
    /// </summary>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// var actionContext = ActionContextInstance.Current;
    /// //Note: actionContext can be null, for instance in an Console Application or a Unit Test Application, or if Mvc is not registered as middleware
    /// </code>
    /// </example>
    public static Microsoft.AspNetCore.Mvc.ActionContext Current => ActionContextAccessor?.ActionContext;
}
