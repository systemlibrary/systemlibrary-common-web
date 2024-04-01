using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

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
    /// Optionally, through the argument CommonWebServicesOptions: 
    /// - Can register view locations
    /// - Can register area view locations
    /// - Can register one ViewLocationExpander
    /// </summary>
    /// <example>
    /// Startup.cs/Program.cs:
    /// <code>
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     var options = new CommonWebServicesOptions();
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
    ///     services.AddCommonWebServices(options);
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddCommonWebServices(this IServiceCollection services, ServicesCollectionOptions options = null)
    {
        if (options == null)
            options = new ServicesCollectionOptions();

        if (options.UseForwardedHeaders)
            services = services.UseForwardedHeaders();

        if (options.UseGzipResponseCompression || options.UseBrotliResponseCompression)
            services = services.UseResponseCompression();

        if (options.UseOutputCache)
            services.AddOutputCache();

        if(options.UseResponseCaching)
            services.AddResponseCaching();

        IMvcBuilder builder = null;

        if(options.UseMvc)
            builder = services.AddMvc();

        if (options.UseRazorPages)
            builder = services.UseAddRazorPages(options);

        if (options.UseControllers)
            builder = services.UseAddControllers(options);

        if (options.UseAutomaticKeyGenerationFile)
            services.UseAutomaticKeyGenerationFile(options);

        if (options.AddApplicationAsPart)
        {
            var executingAssembliy = Assembly.GetCallingAssembly();
            var entryAssembly = Assembly.GetEntryAssembly();
            builder = AddApplicationPart(builder, options, executingAssembliy, entryAssembly);
        }

        if (options.ApplicationParts != null)
        {
            foreach (var part in options.ApplicationParts)
                if (part != null)
                    builder = AddApplicationPart(builder, options, part, null);
        }

        if (options.AddRazorRuntimeCompilationOnChange)
            AddRazorRuntimeCompilationOnChange(builder);

        services = services.UseViews(options);

        if (options.UseCookiePolicy)
            services = services.UseCookiePolicy();

        if (options.AddForwardStandardLogging)
            services.AddLogging(builder =>
            {
                builder.AddProvider(new InternalLogProvider());
            });

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // NOTE: Can this be Scoped instead?
        services.TryAddTransient<HtmlHelperFactory, HtmlHelperFactory>();
        services = services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

        Services.Collection = services;

        return services;
    }
}