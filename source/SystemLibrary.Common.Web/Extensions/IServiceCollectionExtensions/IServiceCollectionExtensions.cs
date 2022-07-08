using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection object
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Configures ServiceCollection in one-line
        /// 
        /// This registered: MVC, RazorPages, Routing, default set of ViewLocations, ForwardedProtocol and ForwardedIp (XForwardedFor) headers and loading Controllers from your Default Assembly (usually your Web Application assembly)
        /// </summary>
        /// <example>
        /// //Inside your Initialization class/Startup class where you have the method "ConfigureServices":
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     services.CommonWebApplicationServices(); //This extension method
        /// }
        /// </code>
        /// </example>
        public static IServiceCollection CommonWebApplicationServices(this IServiceCollection services, ServiceCollectionOptions options = null)
        {
            if (options == null)
                options = new ServiceCollectionOptions();

            services.Configure<ForwardedHeadersOptions>(forwardOption =>
            {
                forwardOption.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor;
            });

            IMvcBuilder builder = null;

            if (options.AddMvcPages)
            {
                builder = services.AddMvc(mvc => {
                    mvc.OutputFormatters.Add(new DefaultSupportedMediaTypes());

                    if (options.StringOutputFormatter != null)
                        mvc.OutputFormatters.Add(options.StringOutputFormatter);
                });
            }

            else if (options.AddRazorPages)
                builder = services.AddRazorPages();

            if (options.AddMvcPages || options.AddRazorPages || options.AddControllers)
            {
                if (builder == null)
                    builder = services.AddControllers();

                var executingAssembliy = Assembly.GetExecutingAssembly();
                var entryAssembly = Assembly.GetEntryAssembly();

                if (executingAssembliy != null)
                    builder.AddApplicationPart(executingAssembliy);

                if (executingAssembliy?.FullName != entryAssembly?.FullName)
                    builder.AddApplicationPart(entryAssembly);
            }

            if (options.AddRazorRuntimeCompilation)
            {
                if (builder != null)
                    builder.AddRazorRuntimeCompilation();
            }

            services.Configure<RazorViewEngineOptions>(razorViews => {
                razorViews.ViewLocationExpanders.Add(new ViewLocations());

                if (options.ViewLocationExpander != null)
                {
                    var views = options.ViewLocationExpander.ExpandViewLocations(null, null);

                    if (views != null)
                    {
                        foreach (var view in views)
                        {
                            if (view.IsNot()) continue;

                            razorViews.ViewLocationFormats.Add(view);
                        }
                    }
                }

                if (options.ViewLocations != null)
                {
                    foreach (var view in options.ViewLocations)
                    {
                        if (view.IsNot()) continue;

                        razorViews.ViewLocationFormats.Add(view);
                    }
                }
            });

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

            return services;
        }
    }
}