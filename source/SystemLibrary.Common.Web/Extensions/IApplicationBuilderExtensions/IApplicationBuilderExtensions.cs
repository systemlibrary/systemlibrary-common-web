using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Extension methods for ApplicationBuilder object
/// </summary>
public static partial class IApplicationBuilderExtensions
{
    /// <summary>
    /// Register common middlewares for a web application
    /// 
    /// Note: This is usually the first registration of middlewares you have, unless your own logging middleware/tracing goes before
    /// 
    /// This will register:
    /// - Http to Https redirection middleware, client and server side
    /// - Routing urls to controllers middleware
    /// - /api/ urls to controllers middleware
    /// - Authentication and Authorization attributes' middleware
    /// - Servince static files such as .css, .js, .jpg, etc... middleware
    /// - Forwarded headers middleware
    /// - Razor pages and Mvc middleware
    /// - Secure cookie policy middleware
    /// - Secure cookie policy (http only middleware
    /// - Recompiling razor pages (saving a cshtml file) middleware
    /// - Exception page middleware
    /// </summary>
    /// <example>
    /// Startup.cs/Program.cs:
    /// <code>
    /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    /// {
    ///     var options = new CommonWebAppOptions();
    ///     
    ///     app.CommonWebApplicationBuilder(options);
    /// }
    /// </code>
    /// </example>
    public static IApplicationBuilder UseCommonWebApp(this IApplicationBuilder app, IWebHostEnvironment env, CommonWebAppOptions options = null)
    {
        if (options == null)
            options = new CommonWebAppOptions();

        if (options.UseDeveloperPage)
            app.UseDeveloperExceptionPage();

        if (options.UseForwardedHeaders)
            app.UseForwardedHeaders();

        if (options.UseHttpToHttpsRedirectionAndHsts)
            app.UseHttpsRedirection();

        if (options.UseHttpToHttpsRedirectionAndHsts)
            app.UseHsts();

        if (options.UseBrotliResponseCompression || options.UseGzipResponseCompression)
            app.UseResponseCompression();

        if (options.UseStaticFiles)
        {
            StaticFileOptions staticFileOptions = new StaticFileOptions
            {
                ServeUnknownFileTypes = options.StaticFileServeUnknownFileTypes,
                HttpsCompression = HttpsCompressionMode.Compress,
                RedirectToAppendTrailingSlash = false,
                OnPrepareResponse = ctx =>
                {
                    if (ctx.Context.Response.Headers.ContainsKey("Cache-Control") != true)
                        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age=" + options.StaticFilesCacheMaxAgeSeconds);
                },
            };

            var dir = env?.WebRootPath ?? Directory.GetCurrentDirectory();

            if (env?.WebRootPath == null)
            {
                if (dir.Contains("\\bin\\") || dir.Contains("/bin/"))
                    dir = Directory.GetParent(dir).FullName;
                if (dir.Contains("\\bin\\") || dir.Contains("/bin/"))
                    dir = Directory.GetParent(dir).FullName;
                if (dir.Contains("\\bin\\") || dir.Contains("/bin/"))
                    dir = Directory.GetParent(dir).FullName;
            }

            staticFileOptions.FileProvider = new PhysicalFileProvider(dir);
            staticFileOptions.RequestPath = new PathString(options.StaticFileRequestPath);

            app.UseStaticFiles(staticFileOptions);
        }

        if (options.UseRouting)
            app.UseRouting();

        if (options.UseCookiePolicy)
        {
            var cookieOptions = new CookiePolicyOptions() { };
            cookieOptions.Secure = CookieSecurePolicy.SameAsRequest;
            cookieOptions.HttpOnly = HttpOnlyPolicy.None;
            cookieOptions.MinimumSameSitePolicy = SameSiteMode.Lax;
            cookieOptions.CheckConsentNeeded = context => false;
            app.UseCookiePolicy(cookieOptions);
        }

        if (options.UseOutputCaching)
            app.UseOutputCache();

        if (options.UseAuthentication)
            app.UseAuthentication();

        if (options.UseOutputCaching && options.UseOutputCacheForAuthenticatedUsers)
            app.UseOutputCache();

        if (options.UseAuthorization)
            app.UseAuthorization();

        if (options.UseRazorPages)
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        if (options.UseControllers)
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

        if (options.UseApiControllers)
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("api/{controller}/{action}/{id?}", "api/{controller}/{action}/{id?}");
            });

        HttpContextInstance.HttpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

        ActionContextInstance.ActionContextAccessor = app.ApplicationServices.GetRequiredService<IActionContextAccessor>();

        Services.ServiceProviderInstance = app.ApplicationServices;

        return app;
    }
}