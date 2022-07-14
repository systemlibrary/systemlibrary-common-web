using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Extension methods for ApplicationBuilder object
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Initialize app with common web application middlewares in one-line
    /// 
    /// All middlewares can be turned on/off through the 'options' variable, by default they are all enabled:
    /// Adds middleware for:
    /// - Http to Https redirection middleware
    /// - Routing middleware
    /// - Authentication and Authorization middleware
    /// - Serving Static files (CSS, jpg, js...) middleware
    /// - Forwarded headers middleware
    /// - Controllers to Endpoints middleware
    /// - RazorPages to Endpoints middleware
    /// - Recompile RazorPage On Saved middleware
    /// - Static file serving middleware
    /// - Use Exception Page middleware
    /// </summary>
    /// <example>
    /// Inside your 'Startup' class:
    /// <code>
    /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    /// {
    ///     app.CommonWebApplicationBuilder();
    /// }
    /// </code>
    /// </example>
    public static IApplicationBuilder CommonWebApplicationBuilder(this IApplicationBuilder app, WebApplicationBuilderOptions options = null)
    {
        if (options == null)
            options = new WebApplicationBuilderOptions();

        if (options.UseExceptionPageInTestAndDev && !EnvironmentConfig.Current.IsProd)
        {
            app.UseDeveloperExceptionPage();
        }

        if (options.UseHttpRedirectionAndHsts)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        if (options.UseDefaultRouting)
        {
            app.UseRouting();
        }

        if (options.UseHttpsAndSecureCookiePolicy)
        {
            app.UseCookiePolicy();
        }

        app.UseForwardedHeaders();

        if (options.UseAuthenticationAndAuthorization)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
        HttpContextInstance.Initialize(httpContextAccessor);

        var actionContextAccessor = app.ApplicationServices.GetRequiredService<IActionContextAccessor>();
        ActionContextInstance.Initialize(actionContextAccessor);

        if (options.UseControllerEndpoints)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllerRoute("api/{controller}/{action}", "api/{controller}/{action}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        if (options.UseRazorPagesEndpoints)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        if (options.UseStaticFiles)
        {
            StaticFileOptions staticFileOptions = new StaticFileOptions();
            staticFileOptions.ServeUnknownFileTypes = true;
            staticFileOptions.HttpsCompression = HttpsCompressionMode.Compress;
            staticFileOptions.RedirectToAppendTrailingSlash = false;

            //TODO: Sure about GetCurrentDirectory? It returns "root" of the application
            //while AppContext.BaseDirectory returns "one folder deeper", inside /bin/, where APP is running
            //but App static files are of course, usually, not copied to bin, so far so good
            staticFileOptions.FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            staticFileOptions.RequestPath = new PathString();

            app.UseStaticFiles(staticFileOptions);
        }


        return app;
    }
}