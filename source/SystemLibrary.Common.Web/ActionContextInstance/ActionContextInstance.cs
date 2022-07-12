using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SystemLibrary.Common.Web
{
    public static class ActionContextInstance
    {
        static IActionContextAccessor ActionContextAccessor;

        public static Microsoft.AspNetCore.Mvc.ActionContext Current => ActionContextAccessor?.ActionContext;

        internal static void Initialize(IActionContextAccessor actionContextAccessor)
        {
            ActionContextAccessor = actionContextAccessor;
        }
    }
}
