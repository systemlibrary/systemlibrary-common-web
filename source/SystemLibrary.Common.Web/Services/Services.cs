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
            {
                _ServiceProvider = HttpContextInstance.Current?.RequestServices;

                if (_ServiceProvider == null)
                    _ServiceProvider = ServiceProviderInstance;
            }

            return _ServiceProvider;
        }
    }

    /// <summary>
    /// Returns the service registered for the type T or null if not found
    /// </summary>
    public static T Get<T>() where T : class
    {
        return ServiceProvider?.GetService<T>();
    }

    /// <summary>
    /// Tries to remove the service registered, or does nothing if already removed
    /// 
    /// Throws exception if called too early in the "middleware pipeline"
    /// </summary>
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