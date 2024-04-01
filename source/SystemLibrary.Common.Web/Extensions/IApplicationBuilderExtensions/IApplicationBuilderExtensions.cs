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
    public static IApplicationBuilder UseBranch(this IApplicationBuilder app, IWebHostEnvironment env, string branch, AppBuilderOptions options = null)
    {
        if (branch == null) throw new System.Exception("A branch cannot be null, either pass a branch like /api, or use the method UseCommonWebApp if you do not want a branch");

        app.MapWhen(context => context?.Request != null && context.Request.Path.Value != null && context.Request.Path.StartsWithSegments(branch), branch =>
        {
            branch.UseCommonWebApp(env, options);
        });

        return app;
    }

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
    public static IApplicationBuilder UseCommonWebApp(this IApplicationBuilder app, IWebHostEnvironment env, AppBuilderOptions options = null)
    {
        if (options == null)
            options = new AppBuilderOptions();

        if (options.UseDeveloperPage)
            app.UseDeveloperExceptionPage();

        if (options.UseForwardedHeaders)
            app.UseForwardedHeaders();

        if (options.UseHttpsRedirection)
            app.UseHttpsRedirection();

        if (options.UseHsts)
            app.UseHsts();

        if (options.UseBrotliResponseCompression || options.UseGzipResponseCompression)
            app.UseResponseCompression();

        if (options.UseStaticFiles)
        {
            StaticFileOptions staticFileOptions = new StaticFileOptions
            {
                ServeUnknownFileTypes = options.StaticFilesServeUnknownFileTypes,
                HttpsCompression = HttpsCompressionMode.Compress,
                RedirectToAppendTrailingSlash = false,
                OnPrepareResponse = ctx =>
                {
                    if (ctx.Context.Response.Headers.ContainsKey("Cache-Control") != true)
                        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age=" + options.StaticFilesMaxAgeSeconds);
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
            staticFileOptions.RequestPath = new PathString(options.StaticFilesRequestPath ?? "");

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

        if (options.UseOutputCache)
            app.UseOutputCache();

        if (options.UseAuthentication)
            app.UseAuthentication();

        if (options.UseOutputCache && options.UseOutputCacheAfterAuthentication)
            app.UseOutputCache();

        if (options.UseAuthorization)
            app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            if(options.UseControllers)
                endpoints.MapDefaultControllerRoute();

            if(options.UseApiControllers)
                endpoints.MapControllerRoute("api/{controller}/{action}/{id?}", "api/{controller}/{action}/{id?}");

            if (options.UseRazorPages)
                endpoints.MapRazorPages();
        });

        HttpContextInstance.HttpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

        ActionContextInstance.ActionContextAccessor = app.ApplicationServices.GetRequiredService<IActionContextAccessor>();

        Services.ServiceProviderInstance = app.ApplicationServices;

        return app;
    }
}