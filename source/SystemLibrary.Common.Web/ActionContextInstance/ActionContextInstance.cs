using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SystemLibrary.Common.Web;

/// <summary>
/// A class that has a reference to the ActionContext instance
/// </summary>
public static class ActionContextInstance
{
    internal static IActionContextAccessor ActionContextAccessor;

    /// <summary>
    /// Returns current Action Context
    /// </summary>
    /// <remarks>
    /// ActionContext can be null in a Console Application or a Unit Test application or if MVC is not registered
    /// </remarks>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// var actionContext = ActionContextInstance.Current;
    /// </code>
    /// </example>
    public static Microsoft.AspNetCore.Mvc.ActionContext Current => ActionContextAccessor?.ActionContext;
}
