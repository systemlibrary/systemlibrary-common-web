//using System.Threading;
//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Http;

//using static SystemLibrary.Common.Web.HttpContextInstance;

//namespace SystemLibrary.Common.Web;


///// <summary>
///// An implementation of the 'old' thread safe singleton HttpContext we all know and love from .NET Framework
///// </summary>
//public static class HttpContextInstance
//{
//    public static HttpContext Current
//    {
//        get
//        {
//            return HttpContextProvider.Current;
//        }
//    }
    
//    internal static class HttpContextProvider
//    {
//        static AsyncLocal<HttpContext> _httpContext = new AsyncLocal<HttpContext>();

//        public static HttpContext Current
//        {
//            get => _httpContext.Value;
//            set => _httpContext.Value = value;
//        }
//    }
//}

//internal class HttpContextProviderMiddleware
//{
//    RequestDelegate _next;

//    public HttpContextProviderMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        HttpContextProvider.Current = context;

//        try
//        {
//            await _next(context);
//        }
//        finally
//        {
//            HttpContextProvider.Current = null;
//        }
//    }
//}