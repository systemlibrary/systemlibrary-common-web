using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Web;

/// <summary>
/// Dependency injection registry
/// 
/// - look at it as 'Service Locator'
/// 
/// - works in unit tests, console app's and web applications
/// 
/// Usage:
/// - requires a call in startup/program to 'services.AddCommonWebServices()' before usage
/// </summary>
/// <example>
/// <code class="language-csharp hljs">
/// public class Car : IVehicle 
/// { 
///     public string Name = "Some car name";
/// }
/// </code>
/// 
/// Startup.cs/Program.cs:
/// <code>
/// public void ConfigureServices(IServiceCollection services)
/// {
///    var options = new CommonWebServicesOptions();
///    
///    services.AddCommonWebServices(options);
///    
///    services.AddTransient&lt;IVehicle, Car&gt;();
/// }
/// </code>
/// 
/// In a controller or anywhere after the initialization of services has ran:
/// <code>
/// var car = Services.Get&gt;IVehicle&lt;
/// car.Name //'car' is constructed so it's not null
/// Dump.Write(car.GetType()); //dumps 'Car', not 'IVehicle'
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

    public static void AddScoped<T,TImpementation>()
        where T : class
        where TImpementation : class, T
    {
        if (Collection == null)
            throw new Exception("You are calling 'Remove()' of " + typeof(T).Name + " too early, call after AddCommonWebServices() has been ran");

        Collection.AddScoped<T, TImpementation>();
    }

    public static void AddSingleton<T, TImpementation>()
        where T : class
        where TImpementation : class, T
    {
        if (Collection == null)
            throw new Exception("You are calling 'Remove()' of " + typeof(T).Name + " too early, call after AddCommonWebServices() has been ran");

        Collection.AddSingleton<T, TImpementation>();
    }

    /// <summary>
    /// Tries to remove the service registered, or does nothing if already removed
    /// 
    /// Note: can throw exception if it is invoked on too early in the 'middleware pipeline'
    /// </summary>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// Services.Remove&lt;IVehicle&gt;();
    /// // Removes IVehicle if it exists
    /// // If it does not exist, it calls Log.Error() with message, it does not throw exception
    /// 
    /// // Note: It can throw exception if called before services.AddCommonWebServices()
    /// </code>
    /// </example>
    public static void Remove<T>()
    {
        if (Collection == null)
            throw new Exception("You are calling 'Remove()' of " + typeof(T).Name + " too early, call after AddCommonWebServices() has been ran");

        var type = typeof(T);
        if (type.IsClass || type.IsInterface)
        {
            Collection.RemoveAll<T>();
        }
        else
            Log.Error(typeof(T).Name + " is not an interface nor a class, cannot be removed");
    }
}