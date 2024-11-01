using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Web;

public static class MetricsAuthorizationMiddleware
{
    public static bool AuthorizeMetricsRequest(HttpContext context)
    {
        var authorizationValue = AppSettings.Current.SystemLibraryCommonWeb.Metrics.AuthorizationValue;
        var authorization = context.Request.Headers["Authorization"].ToString();

        if(authorizationValue.IsNot() || "Basic " + authorizationValue == authorization)
        {
            return true;
        }

        context.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.WriteAsync(StatusCodes.Status401Unauthorized.ToString())
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        return false;
    }
}