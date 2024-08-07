﻿using System;
using System.ComponentModel;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static partial class IServiceCollectionExtensions
{
    /// <summary>
    /// Configures ServiceCollection in one-line, so register all of your own or other service configurations after this one
    /// <list>
    /// <item>Registers:</item>
    /// <item>- MVC services</item>
    /// <item>- Razor Page services</item>
    /// <item>- Routing services</item>
    /// <item>- ForwardedProtocol and ForwardedIp (XForwardedFor) headers</item>
    /// <item>- Compression for Gzip and Brotli services</item>
    /// <item>- Authentication and authorization services</item>
    /// <item>- Output cache services</item>
    /// <item>- Registers the main assembly and all its controllers (if any), as in: your Web Application Project's assembly</item>
    /// </list>
    /// Optionally, through the argument ServicesCollectionOptions: 
    /// <list>
    /// <item>- Can register view locations</item>
    /// <item>- Can register area view locations</item>
    /// <item>- Can register one ViewLocationExpander</item>
    /// <item>- and more...</item>
    /// </list>
    /// </summary>
    /// <example>
    /// Startup.cs/Program.cs:
    /// <code>
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     var options = new ServicesCollectionOptions();
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
        Services.Configure(services);

        options ??= new ServicesCollectionOptions();

        if (options.UseCustomTypeConverters)
        {
            var enumType = typeof(Enum);
            var converters = TypeDescriptor.GetConverter(enumType);
            if (converters == null || !(converters is GlobalEnumConverter))
            {
                TypeDescriptor.AddAttributes(enumType, new TypeConverterAttribute(typeof(GlobalEnumConverter)));
            }
        }

        if (options.UseForwardedHeaders)
            services = services.UseForwardedHeaders();

        if (options.UseHttpsRedirection)
            services.AddHttpsRedirection(opt => opt.HttpsPort = 443);

        if (options.UseGzipResponseCompression)
            services = services.UseGzipCompression();

        if (options.UseBrotliResponseCompression)
            services = services.UseBrotliCompression();

        if (options.UseOutputCache)
            services.AddOutputCache();

        if (options.UseResponseCaching)
            services.AddResponseCaching();

        services.UseAutomaticDataProtectionPolicy(options);

        IMvcBuilder builder = null;

        if (options.UseMvc)
            builder = services.AddMvc();

        if (options.UseRazorPages)
            builder = services.UseAddRazorPages(options);

        if (options.UseControllers)
            builder = services.UseAddControllers(options);

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

        if (options.IISAllowSynchronousIO)
            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

        return services;
    }

    /// <summary>
    /// Configures ServiceCollection in one-line, so register all of your own or other service configurations after this one
    /// <list>
    /// <item>Registers:</item>
    /// <item>- MVC services</item>
    /// <item>- Razor Page services</item>
    /// <item>- Routing services</item>
    /// <item>- ForwardedProtocol and ForwardedIp (XForwardedFor) headers</item>
    /// <item>- Compression for Gzip and Brotli services</item>
    /// <item>- Authentication and authorization services</item>
    /// <item>- Output cache services</item>
    /// <item>- Registers the main assembly and all its controllers (if any), as in: your Web Application Project's assembly</item>
    /// </list>
    /// Optionally, through the argument ServicesCollectionOptions: 
    /// <list>
    /// <item>- Can register view locations</item>
    /// <item>- Can register area view locations</item>
    /// <item>- Can register one ViewLocationExpander</item>
    /// <item>- and more...</item>
    /// </list>
    /// </summary>
    /// <example>
    /// Startup.cs/Program.cs:
    /// <code>
    /// pulic class LogWriter : ILogWriter 
    /// {
    ///     // Implement interface methods
    /// }
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     var options = new ServicesCollectionOptions();
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
    ///     services.AddCommonWebServices&lt;LogWriter&gt;(options);
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddCommonWebServices<TLogWriter>(this IServiceCollection services, ServicesCollectionOptions options = null) where TLogWriter : class, ILogWriter
    {
        services = AddCommonWebServices(services, options);

        // NOTE: Was transient, now scoped, but probably can be a singleton as it gets all input
        services = services.AddScoped<ILogWriter, TLogWriter>();

        return services;
    }
}