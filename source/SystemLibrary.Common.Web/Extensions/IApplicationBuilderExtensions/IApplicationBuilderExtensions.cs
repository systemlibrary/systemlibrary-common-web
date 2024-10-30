using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using Prometheus;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Configurations;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Extension methods for ApplicationBuilder object
/// </summary>
public static partial class IApplicationBuilderExtensions
{
    /// <summary>
    /// Specify a path as a branch to register different middlewares to trigger on certain paths
    /// </summary>
    public static IApplicationBuilder UseBranch(this IApplicationBuilder app, IWebHostEnvironment env, string branchPath, AppBuilderOptions options = null)
    {
        if (branchPath == null) throw new System.Exception("A branch cannot be null, either pass a branch path like '/api', or use the method UseCommonWebApp if you do not want a branch");

        app.MapWhen(context => context?.Request != null && context.Request.Path.Value != null && context.Request.Path.StartsWithSegments(branchPath), branch =>
        {
            branch.UseCommonWebApp(env, options);
        });

        return app;
    }

    /// <summary>
    /// Register common middlewares for a web application
    /// <para>This will register:</para>
    /// - Http to Https redirection middleware, client and server side
    /// <para>- Routing urls to controllers middleware</para>
    /// - /api/ urls to controllers middleware
    /// <para>- Authentication and Authorization attributes' middleware</para>
    /// - Servince static files such as .css, .js, .jpg, etc... middleware
    /// <para>- Forwarded headers middleware</para>
    /// - Razor pages and Mvc middleware
    /// <para>- Secure cookie policy middleware</para>
    /// - Secure cookie policy (http only middleware
    /// <para>- Recompiling razor pages (saving a cshtml file) middleware</para>
    /// - Exception page middleware
    /// </summary>
    /// <remarks>
    /// This should be your first registration of middlewares you have, exception might be your own middleware for logging/tracing requests
    /// </remarks>
    /// <example>
    /// Startup.cs/Program.cs:
    /// <code>
    /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    /// {
    ///     var options = new AppBuilderOptions();
    ///     
    ///     app.CommonWebApplicationBuilder(options);
    /// }
    /// </code>
    /// </example>
    public static IApplicationBuilder UseCommonWebApp(this IApplicationBuilder app, IWebHostEnvironment env, AppBuilderOptions options = null)
    {
        Services.Configure(app.ApplicationServices);

        options ??= new AppBuilderOptions();

        if (options.UseDeveloperPage)
            app.UseDeveloperExceptionPage();

        if (options.UseForwardedHeaders)
            app.UseForwardedHeaders();

        if (options.UseHsts)
            app.UseHsts();

        if (options.UseHttpsRedirection)
            app.UseHttpsRedirection();

        if (options.UseStaticFiles)
        {
            var contentRootPath = env?.WebRootPath ?? EnvironmentConfig.Current.ContentRootPath;

            if (options.StaticFilesRequestPaths.Is())
            {
                foreach (var staticFilePath in options.StaticFilesRequestPaths)
                {
                    if (staticFilePath == null) continue;

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

                    staticFileOptions.FileProvider = new PhysicalFileProvider(contentRootPath);
                    staticFileOptions.RequestPath = new PathString(staticFilePath);
                    app.UseStaticFiles(staticFileOptions);
                }
            }
            else
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
                staticFileOptions.FileProvider = new PhysicalFileProvider(contentRootPath);
                staticFileOptions.RequestPath = new PathString("");
                app.UseStaticFiles(staticFileOptions);
            }
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

        if (options.UseOutputCache && !options.UseOutputCacheAfterAuthentication)
            app.UseOutputCache();

        if (!options.UseOutputCacheAfterAuthentication)
        {
            if (options.UseBrotliResponseCompression || options.UseGzipResponseCompression)
            {
                app.UseWhen((context) => Compress.IsEligibleForCompression(context, options), appCompression =>
                {
                    appCompression.UseResponseCompression();
                });
            }
        }

        if (options.UseAuthentication)
            app.UseAuthentication();

        if (options.UseOutputCache && options.UseOutputCacheAfterAuthentication)
            app.UseOutputCache();

        if (options.UseOutputCacheAfterAuthentication)
        {
            if (options.UseBrotliResponseCompression || options.UseGzipResponseCompression)
            {
                app.UseWhen((context) => Compress.IsEligibleForCompression(context, options), appCompression =>
                {
                    appCompression.UseResponseCompression();
                });
            }
        }

        if (options.UseAuthorization)
            app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            if (options.UseRazorPages)
                endpoints.MapRazorPages();

            if (options.UseControllers)
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            }

            if (options.UseApiControllers)
                endpoints.MapControllerRoute("api/{controller}/{action}/{id?}", "api/{controller}/{action}/{id?}");
        });

        var enablePrometheusMetrics = AppSettings.Current.SystemLibraryCommonWeb.Metrics.EnablePrometheus;
        if (enablePrometheusMetrics)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Equals("/metrics", System.StringComparison.OrdinalIgnoreCase) ||
                    context.Request.Path.Equals("/metrics/", System.StringComparison.OrdinalIgnoreCase))
                {
                    var authorizationValue = AppSettings.Current?.SystemLibraryCommonWeb?.Metrics?.AuthorizationValue;
                    var authorization = context.Request.Headers["Authorization"].ToString();

                    if (authorizationValue == null || "Basic " + authorizationValue == authorization)
                    {
                        await next();
                        return;
                    }

                    context.Response.Headers["WWW-Authenticate"] = "Basic";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                else
                {
                    await next();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
            });
        }

        HttpContextInstance.HttpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

        ActionContextInstance.ActionContextAccessor = app.ApplicationServices.GetRequiredService<IActionContextAccessor>();

        return app;
    }
}