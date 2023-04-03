using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Extension methods for IServiceCollection object
/// </summary>
public static partial class IServiceCollectionExtensions
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

        services = services.UseForwardedHeaders();

        IMvcBuilder builder = null;

        if (options.AddMvc)
        {
            builder = services.UseAddMvc(options);
        }
        else if (options.AddRazorPages)
        {
            builder = services.UseAddRazorPages(options);
        }
        else if (options.AddControllers)
            builder = services.UseAddControllers(options);

        builder = AddApplicationPart(builder, options);

        if (options.AddRazorRecompilationOnViewChanged)
        {
            AddRazorRuntimeRecompilation(builder);
        }

        services = services.UseViews(options);

        if (options.AddHttpsAndSecureCookiePolicy)
        {
            services = services.UseCookiePolicy();
        }

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.TryAddTransient<HtmlHelperFactory, HtmlHelperFactory>();
        services = services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

        Services.Collection = services;

        return services;
    }
}