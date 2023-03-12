using System;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    /// Note: register all of your own services after this one is called
    /// 
    /// Registers:
    /// - Aspnet Mvc services
    /// - Razor Pages services
    /// - Routing (requests to controllers mapping)
    /// - ForwardedProtocol and ForwardedIp (XForwardedFor) headers 
    /// - Registering Controllers in your Startup Assembly - usually your Web Application Project's assembly
    /// 
    /// Optionally, through the argument CommonWebApplicationServicesOptions: 
    /// - Can register view locations
    /// - Can register area view locations
    /// - Can register one ViewLocationExpander
    /// </summary>
    /// <example>
    /// Startup.cs/Program.cs:
    /// <code>
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     var options = new CommonWebApplicationServicesOptions();
    ///     
    ///     options.ViewLocations = new string[] {
    ///         "~/Views/{0}/index.cshtml"
    ///     }
    ///     
    ///     options.AreaViewLocations = new string[] {
    ///         "~/Area/{2}/{1}/{0}.cshtml"
    ///     }
    ///     
    ///     options.ViewLocationExpander = null; //or create one based on the Interface 'IViewLocationExpander'
    /// 
    ///     services.CommonWebApplicationServices(options);
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection CommonWebApplicationServices(this IServiceCollection services, CommonWebApplicationServicesOptions options = null)
    {
        if (options == null)
            options = new CommonWebApplicationServicesOptions();

        services = services.Configure<ForwardedHeadersOptions>(forwardOption =>
        {
            forwardOption.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor;
        });

        IMvcBuilder builder = null;

        if (options.AddMvc)
        {
            builder = services.AddMvc(ConfigureMvcOptions(options));
        }

        else if (options.AddRazorPages)
        {
            builder = services.AddRazorPages();
            builder.Services.Configure(ConfigureMvcOptions(options));
        }

        else if (options.AddControllers)
            builder = services.AddControllers(ConfigureMvcOptions(options));

        if (builder != null)
        {
            var executingAssembliy = Assembly.GetExecutingAssembly();
            var entryAssembly = Assembly.GetEntryAssembly();

            if (executingAssembliy != null)
                builder.AddApplicationPart(executingAssembliy);

            if (executingAssembliy?.FullName != entryAssembly?.FullName)
                builder.AddApplicationPart(entryAssembly);
        }
        
        else if(options.SupportedMediaTypes != null)
            throw new Exception("AddMvcPages, AddRazorPages and AddControllers are false, yet you've set SupportedMediaTypes. Either set one of the flags to true, or register SupportedMediaTypes yourself");

        if (options.AddRazorRuntimeReCompilationOnViewChanged)
        {
            if (builder != null)
                builder.AddRazorRuntimeCompilation();
        }

        services = services.Configure<RazorViewEngineOptions>(razorViews =>
        {
            if (options.ViewLocationExpander != null)
                razorViews.ViewLocationExpanders.Add(options.ViewLocationExpander);

            if(options.AreaViewLocations != null)
            {
                foreach(var view in options.AreaViewLocations)
                {
                    if (view.IsNot()) continue;

                    razorViews.AreaViewLocationFormats.Add(view);
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

        if (options.AddHttpsAndSecureCookiePolicy)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = HttpOnlyPolicy.Always;

                options.Secure = CookieSecurePolicy.SameAsRequest;

                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });
        }

        services = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
            .AddTransient<HtmlHelperFactory, HtmlHelperFactory>()
            .Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

        Services.Collection = services;

        return services;
    }

    static Action<MvcOptions> ConfigureMvcOptions(CommonWebApplicationServicesOptions options)
    {
        return mvc =>
        {
            mvc.OutputFormatters.Add(new DefaultSupportedMediaTypes());

            if (options.SupportedMediaTypes != null)
                mvc.OutputFormatters.Add(options.SupportedMediaTypes);
        };
    }
}