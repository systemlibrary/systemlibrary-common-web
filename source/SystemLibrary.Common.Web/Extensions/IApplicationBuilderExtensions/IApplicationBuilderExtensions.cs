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
        /// Registers:
        /// Http to Https redirection, usage of Authentication and Authorization, Static File Handlers, Forwarded Headers and Endpoints for RazorPage and Controllers
        /// </summary>
        /// <example>
        /// Inside your 'Startup' class:
        /// <code>
        /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        /// {
        ///     app.CommonAppBuilder();
        /// }
        /// </code>
        /// </example>
        public static IApplicationBuilder CommonAppBuilder(this IApplicationBuilder app, ApplicationBuilderOptions options = null)
        {
            if (options == null)
                options = new ApplicationBuilderOptions();

            if (options.UseHttpRedirectionAndHsts)
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            if (options.UseRouting)
                app.UseRouting();

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
                        name: "default-controller-action-mapping",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });
            }

            return app;
        }
    }
}