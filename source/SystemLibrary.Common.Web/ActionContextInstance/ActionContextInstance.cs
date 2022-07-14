using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SystemLibrary.Common.Web;

/// <summary>
/// An implementation of the 'old' thread safe singleton ActionContext we all know and love from .NET Framework
/// </summary>
public static class ActionContextInstance
{
    static IActionContextAccessor ActionContextAccessor;

    /// <summary>
    /// Returns current Action Context
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ActionContext Current => ActionContextAccessor?.ActionContext;

    internal static void Initialize(IActionContextAccessor actionContextAccessor)
    {
        ActionContextAccessor = actionContextAccessor;
    }
}
