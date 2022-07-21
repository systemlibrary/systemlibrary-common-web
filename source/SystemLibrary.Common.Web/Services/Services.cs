using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Web;

/// <summary>
/// Dependency injection registry with all services
/// - Look at it as ServiceLocator
/// - Works both in Unit Tests (console app's) and in your Web Application
/// 
/// Note: Requires a call in your Startup services.CommonWebApplicationServices() before it can be used
/// </summary>
/// <example>
/// <code class="language-csharp hljs">
/// public class Car : IVehicle 
/// { 
///     public string Name = "Some car name";
/// }
/// 
/// //Inside your startup/initialize class
/// public void ConfigureServices(IServiceCollection services)
/// {
///    var options = new CommonWebApplicationServicesOptions();
///    services.CommonWebApplicationServices(options);
///    services.AddTransient&lt;IVehicle, Car&gt;();
/// }
/// 
/// //Anywhere in your solution, for instance in a Controller? In a view? In ...
/// var car = Services.Get&gt;IVehicle&lt;
/// car.Name //car is not null, it has been constructed for you, so this is allowed
/// </code>
/// </example>
public static class Services
{
    internal static IServiceCollection Collection;

    internal static IServiceProvider ServiceProviderInstance;

    static IServiceProvider _ServiceProvider;

    static IServiceProvider ServiceProvider
    {
        get
        {
            if (_ServiceProvider == null)
                _ServiceProvider = ServiceProviderInstance;

            //Commented out: Should never use the HttpContextInstance with RequestServices - as it might be null/disposed there depending on "when" we use it
            //if (_ServiceProvider == null)
            //    _ServiceProvider = HttpContextInstance.Current.RequestServices;

            return _ServiceProvider;
        }
    }

    /// <summary>
    /// Returns the service registered for the type T or null if not found
    /// </summary>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// var obj = Services.Get&lt;IVehicle&gt;();
    /// //obj is now null if IVehicle is not a registered service, otherwise it is new'd up and ready to be used
    /// </code>
    /// </example>
    public static T Get<T>() where T : class
    {
        return ServiceProvider?.GetService<T>();
    }

    /// <summary>
    /// Tries to remove the service registered, or does nothing if already removed
    /// 
    /// Throws exception if called too early in the "middleware pipeline"
    /// </summary>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// Services.Remove&lt;IVehicle&gt;();
    /// // Removes IVehicle if it exists
    /// // If it does not exist, it calls Log.Error() with message, it does not throw exception
    /// 
    /// // Note: It can throw exception if called before services.CommonWebApplicationServices()
    /// </code>
    /// </example>
    public static void Remove<T>()
    {
        if (Collection == null)
            throw new Exception("You are calling 'Remove()' of " + typeof(T).Name + " too early, call after CommonEpiserverServices() has been ran");

        var type = typeof(T);
        if (type.IsClass || type.IsInterface)
        {
            Collection.RemoveAll<T>();
        }
        else
            Log.Error(typeof(T).Name + " is not an interface nor a class, cannot be removed");
    }
}