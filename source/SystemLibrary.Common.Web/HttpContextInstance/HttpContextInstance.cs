using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Web
{
    public static class HttpContextInstance
    {
        static IHttpContextAccessor HttpContextAccessor;

        public static HttpContext Current => HttpContextAccessor?.HttpContext;

        internal static void Initialize(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
    }
}
