//using System.Threading;
//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Infrastructure;

//using static SystemLibrary.Common.Web.ActionContextInstance;

//namespace SystemLibrary.Common.Web;

///// <summary>
///// An implementation of the 'old' thread safe singleton ActionContext we all know and love from .NET Framework
///// </summary>
//public static class ActionContextInstance
//{
//    /// <summary>
//    /// Get the current Action Context instance
//    /// </summary>
//    /// <remarks>
//    /// Do note that Action Context can be null in a console application or in a 'Unit' Test Application or if MVC is not yet registered as a service/invoked
//    /// </remarks>
//    /// <example>
//    /// <code>
//    /// var actionContext = ActionContextInstance.Current;
//    /// </code>
//    /// </example>
//    /// <return>Returns current Action Context or null if there is none</return>
//    public static ActionContext Current
//    {
//        get
//        {
//            return ActionContextProvider.Current;
//        }
//    }


//    internal static class ActionContextProvider
//    {
//        static AsyncLocal<ActionContext> _actionContext = new AsyncLocal<ActionContext>();

//        public static ActionContext Current
//        {
//            get => _actionContext.Value;
//            set => _actionContext.Value = value;
//        }
//    }
//}

//internal class ActionContextProviderMiddleware
//{
//    RequestDelegate _next;

//    public ActionContextProviderMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task InvokeAsync(HttpContext context, IActionContextAccessor actionContextAccessor)
//    {
//        ActionContextProvider.Current = actionContextAccessor.ActionContext;

//        try
//        {
//            await _next(context);
//        }
//        finally
//        {
//            ActionContextProvider.Current = null;
//        }
//    }
//}