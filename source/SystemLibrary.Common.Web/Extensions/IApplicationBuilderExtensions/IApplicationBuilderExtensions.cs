using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

namespace SystemLibrary.Common.Web.Extensions
{
    /// <summary>
    /// Extension methods for ApplicationBuilder object
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Register a common web application builder in one-line.
        /// 
        /// This adds various middleware to your ApplicationBuilder:
        /// - Https middleware
        /// - Http to Https redirection middleware
        /// - Routing middleware
        /// - Authentication and Authorization middleware
        /// - Serving Static files (CSS, jpg, js...) middleware
        /// - Forwarded headers middleware
        /// - Controllers middleware to run your Controllers based on Routing
        /// - RazorPages middleware to compile your Razor Views to serve the HTML
        /// - Default ControllerRoute registered to: controller=Home,action=Index
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
        public static IApplicationBuilder CommonWebApplicationBuilder(this IApplicationBuilder app, ApplicationBuilderOptions options = null)
        {
            if (options == null)
                options = new ApplicationBuilderOptions();

            if (options.UseHttpRedirectionAndHsts)
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            if (options.UseDefaultRouting)
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapControllerRoute("api/{controller}/{action}", "api/{controller}/{action}/{id?}");
                });
            }

            if (options.UseAuthenticationAndAuthorization)
            {
                app.UseAuthentication();

                app.UseAuthorization();
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

            app.UseForwardedHeaders();

            if (options.UseControllerAndRazorPages)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    endpoints.MapRazorPages();

                    endpoints.MapControllerRoute(
                        name: "systemlibrary-common-web-default-controller-action-id",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });
            }

            return app;
        }
    }
}