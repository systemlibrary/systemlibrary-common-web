using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Extension methods for IServiceCollection object
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Configures ServiceCollection in one-line
    /// 
    /// This registers: MVC, RazorPages, Routing, default set of ViewLocations for 'Components' only, ForwardedProtocol and ForwardedIp (XForwardedFor) headers and loading Controllers from your Default Assembly (usually your Web Application assembly)
    /// </summary>
    /// <example>
    /// //Inside your Initialization class/Startup class where you have the method "ConfigureServices":
    /// <code>
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     var options = new CommonWebApplicationServicesOptions();
    ///     services.CommonWebApplicationServices(options);
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection CommonWebApplicationServices(this IServiceCollection services, CommonWebApplicationServicesOptions options = null)
    {
        Services.Collection = services;

        if (options == null)
            options = new CommonWebApplicationServicesOptions();

        services.Configure<ForwardedHeadersOptions>(forwardOption =>
        {
            forwardOption.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor;
        });

        IMvcBuilder builder = null;

        if (options.AddMvcPages)
        {
            builder = services.AddMvc(mvc =>
            {
                mvc.OutputFormatters.Add(new DefaultSupportedMediaTypes());

                if (options.SupportedMediaTypes != null)
                    mvc.OutputFormatters.Add(options.SupportedMediaTypes);
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

        if (options.AddRazorRuntimeReCompilationOnViewChanged)
        {
            if (builder != null)
                builder.AddRazorRuntimeCompilation();
        }

        services.Configure<RazorViewEngineOptions>(razorViews =>
        {
            razorViews.ViewLocationExpanders.Add(new ViewLocations());

            if (options.ViewLocationExpander != null)
                razorViews.ViewLocationExpanders.Add(options.ViewLocationExpander);

            if (options.ViewLocations != null)
            {
                foreach (var view in options.ViewLocations)
                {
                    if (view.IsNot()) continue;

                    razorViews.ViewLocationFormats.Add(view);
                }
            }
        });

        if (options.AddHttpsAndSecureCookiePolicy)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = HttpOnlyPolicy.Always;

                options.Secure = CookieSecurePolicy.SameAsRequest;

                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });
        }

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddTransient<HtmlHelperFactory, HtmlHelperFactory>();

        services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

        return services;
    }
}