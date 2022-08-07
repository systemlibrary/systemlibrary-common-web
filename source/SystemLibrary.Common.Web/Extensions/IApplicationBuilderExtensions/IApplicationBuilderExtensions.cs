using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Extension methods for ApplicationBuilder object
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Register common middlewares for a web application
    /// 
    /// Note: register all of your own middlewares after this one is called
    /// 
    /// This will register:
    /// - Http to Https redirection middleware, client side and server side
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
    ///     var options = new CommonWebApplicationBuilderOptions();
    ///     
    ///     app.CommonWebApplicationBuilder(options);
    /// }
    /// </code>
    /// </example>
    public static IApplicationBuilder CommonWebApplicationBuilder(this IApplicationBuilder app, CommonWebApplicationBuilderOptions options = null)
    {
        Services.ServiceProviderInstance = app.ApplicationServices;

        if (options == null)
            options = new CommonWebApplicationBuilderOptions();

        if (options.UseExceptionPageInTestAndDev)
        {
            app.UseDeveloperExceptionPage();
        }

        if (options.UseHttpToHttpsRedirectionAndHsts)
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
            var cookieOptions = new CookiePolicyOptions() { };
            cookieOptions.Secure = CookieSecurePolicy.SameAsRequest;
            cookieOptions.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
            cookieOptions.MinimumSameSitePolicy = SameSiteMode.Strict;
            app.UseCookiePolicy(cookieOptions);
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
                endpoints.MapControllerRoute("api/{controller}/{action}/{id?}", "api/{controller}/{action}/{id?}");
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