using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
        /// Configures ServiceCollection to be used with Mvc, RazorPages, Routing, ViewLocations, ForwardedHeaders, configures which Media Types are allowed to be hosted as Static Files (css, js, etc...)
        /// </summary>
        /// <example>
        /// //the familiar 'startup.cs':
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     app.CommonServices();
        /// }
        /// </code>
        /// </example>
        public static IServiceCollection CommonServices(this IServiceCollection services, ServiceCollectionOptions options = null)
        {
            if (options == null)
                options = new ServiceCollectionOptions();

            services.Configure<ForwardedHeadersOptions>(forwardOption =>
            {
                forwardOption.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor;
            });

            if (options.AddControllers)
            {
                services.AddControllers()
                    .AddApplicationPart(Assembly.GetExecutingAssembly())
                    .AddApplicationPart(Assembly.GetEntryAssembly());
            }

            //TODO: This is most likely wrong, never used 'RazorPages' nor 'MvcPages' in .NET 5/6...
            //So which views goes where, and what, no clue, just registering something to begin with...
            if (options.AddMvcPages)
            {
                var mvc = services.AddMvc(mvc =>
                {
                    mvc.OutputFormatters.Add(new DefaultSupportedMediaTypes());
                    if (options.StringOutputFormatter != null)
                        mvc.OutputFormatters.Add(options.StringOutputFormatter);
                });

                if (!options.AddRazorPages)
                {
                    mvc.AddRazorOptions(razor =>
                    {
                        razor.ViewLocationExpanders.Add(new ViewLocations());
                        if (options.ViewLocations != null)
                            razor.ViewLocationExpanders.Add(options.ViewLocations);
                    });
                }
            }

            if (options.AddRazorPages)
            {
                services.AddRazorPages()
                    .AddRazorOptions(razor =>
                    {
                        razor.ViewLocationExpanders.Add(new ViewLocations());
                        if (options.ViewLocations != null)
                            razor.ViewLocationExpanders.Add(options.ViewLocations);
                    });
            }

            if (!options.AddMvcPages && !options.AddRazorPages)
            {
                var defaultViewLocations = new ViewLocations();
                var defaultViews = defaultViewLocations.ExpandViewLocations(null, null);

                services.Configure<RazorViewEngineOptions>(o =>
                {
                    foreach (var view in defaultViews)
                    {
                        o.ViewLocationFormats.Add(view);
                    }
                });
                if (options.ViewLocations != null)
                {
                    var views = options.ViewLocations.ExpandViewLocations(null, null);

                    services.Configure<RazorViewEngineOptions>(o =>
                    {
                        foreach (var view in views)
                        {
                            o.ViewLocationFormats.Add(view);
                        }
                    });
                }
            }

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
            return services;
        }
    }
}