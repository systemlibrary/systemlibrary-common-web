using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

using Microsoft.Extensions.Caching.Memory;

using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Web;

/// <summary>
/// Caching for web applications
///
/// Default duration is 180 seconds
/// - configurable in appSettings.json
/// 
/// Auto-generates a cachekey for you if you dont specify one
/// - works then on per role and isAuthenticated level, never per user
///
/// Required skip conditions:
/// - if not in a web context
///
/// Optional skip cache scenarios by paramters can be applied
/// - skipForAuthenticatedUsers
/// - skipForAdmins
///     * admins are if current principal is in either of these roles "Admins", "Administrators", "CmsAdmins" or "WebAdmins", case sensitive
/// - skipFor, your own implementation which returns true to skip
/// </summary>
/// <example>
/// Configure the cache in appSettings.json 
/// <code class="language-csharp hljs">
/// {
///     "systemLibraryCommonWeb": {
///         "cache" { 
///             "defaultDuration": 180,
///         }
///     }
/// }
/// </code>
/// </example>
public static class Cache
{
    static IPrincipal _Principal;
    static IPrincipal Principal => _Principal != null ? _Principal :
        (_Principal = HttpContextInstance.Current?.User);

    static IMemoryCache cache;
    static object cacheLock = new object();

    static int _DefaultDuration = -1;
    static int DefaultDuration
    {
        get
        {
            if (_DefaultDuration == -1)
            {
                _DefaultDuration = AppSettings.Current.SystemLibraryCommonWeb.Cache.DefaultDuration;
                if (_DefaultDuration <= 0)
                {
                    _DefaultDuration = 180;
                }
            }
            return _DefaultDuration;
        }
    }

    static Cache()
    {
        MemoryCacheOptions options = new MemoryCacheOptions();
        options.ExpirationScanFrequency = TimeSpan.FromSeconds(120);
        cache = new MemoryCache(options);
    }

    /// <summary>
    /// Get data from cache as T, or default T if it does not exist in cache or if you are not in a web-context
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var cacheKey = "hello-world-key";
    /// 
    /// var data = Cache.Get&gt;string&lt;(cacheKey);
    /// 
    /// //If 'hello-world-key' exists in cache, the variable 'data' now holds that value
    /// </code>
    /// </example>
    public static T Get<T>(string cacheKey) where T : class
    {
        if (cacheKey.IsNot()) return default;

        return cache.Get(cacheKey) as T;
    }

    /// <summary>
    /// Get data from cache, or add it to cache before it is returned
    /// 
    /// Note: null is never added to cache
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var cacheKey = "hello-world-key";
    /// var data = Cache.Get(() => {
    ///     return "hello world";
    /// });
    /// //'data' is now 'hello world', if called multiple times within the default cache duration of 180 seconds, "hello world" is returned from the cache for all non-admin users
    /// 
    /// //Another example with more options:
    /// var cacheKey = "hello-world-key";
    /// var data = Cache.Get(() => {
    ///         return "hello world";
    ///     },
    ///     cacheKey: cacheKey,
    ///     duration: TimeSpan.FromSeconds(1),
    ///     condition: x => x != "hello world",
    ///     skipForAuthenticatedUsers: false);
    ///     
    /// //'data' is equal to 'hello world', cache duration is 1 second, but it only adds the result to cache, if it is not equal to "hello world"
    /// //So in this scenario - "hello world" is never added to cache, and our function that returns "hello world" is always invoked
    /// </code>
    /// </example>
    /// <param name="condition">Add to cache only if condition is met, for instance: data != null</param>
    /// <param name="skipForAuthenticatedUsers">Skip cache for any user that is authenticated, but is not part of any of the admin roles: Admins, Administrators, WebAdmins, CmsAdmins</param>
    /// <param name="skipForAdmins">Skip cache for current principal that is authenticated and is part of either of the roles: Admins, Administrators, WebAdmins, CmsAdmins</param>
    /// <param name="skipFor">Implement your own logic for when to skip cache, let it return true on your conditions to avoid caching</param>
    /// <returns>Returns data either from cache or the getItem() method</returns>
    public static T Get<T>(Func<T> getItem, string cacheKey = null, TimeSpan duration = default, Func<T, bool> condition = null, bool skipForAuthenticatedUsers = false, bool skipForAdmins = true, Func<bool> skipFor = null, bool debug = false) where T : class
    {
        if (SkipCache(skipForAuthenticatedUsers, skipForAdmins, skipFor))
            return getItem();

        if (duration == default)
            duration = TimeSpan.FromSeconds(DefaultDuration);

        if (cacheKey == null)
            cacheKey = CreateCacheKey(getItem, condition);

        if (debug)
            Log.Debug("Cache.Get() debug flag=true: cache key is " + cacheKey);

        var cached = cache.Get(cacheKey) as T;

        if (cached != null)
        {
            if (debug)
                Log.Debug("Cache.Get() debug flag=true: item is returned from cache");
            return cached;
        }

        cached = getItem();

        if (cached != null && (condition == null || condition(cached)))
            Insert(cacheKey, cached, duration);

        return cached;
    }

    static string CreateCacheKey<T>(Func<T> getItem, Func<T, bool> condition) where T : class
    {
        var key = new StringBuilder("");
        var getItemMethod = getItem.Method;

        key.Append(getItemMethod.Name + getItemMethod.DeclaringType?.FullName + getItemMethod.ReturnType?.FullName);

        //getItem.GetInvocationList()[0].Target - about 3-4 times slower than getItem.Target
        var target = getItem.Target;
        if(target != null)
        {
            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            if(fields.Length > 0)
            {
                foreach (var field in fields)
                    key.Append(field.GetValue(target) + "");
            }
        }
        
        if (condition != null)
            key.Append(condition.Method?.Name + condition.Method?.ReturnType?.Name + "");

        var isAuthenticated = Principal?.Identity?.IsAuthenticated == true;

        if (isAuthenticated)
        {
            key.Append(isAuthenticated);

            if (Principal is ClaimsPrincipal claimsPrincipal)
            {
                var claimsIdentity = claimsPrincipal?.Identity as ClaimsIdentity;

                if (claimsIdentity != null)
                {
                    var roles = claimsPrincipal.Claims
                        .Where(c => c.Type == claimsIdentity.RoleClaimType)
                        .Select(x => x.Value);

                    if (roles != null)
                        key.Append(string.Join("", roles));
                }
            }
            else
            {
                key.Append(IsCurrentUserAdmin());
            }
        }
        return key.ToString();
    }

    static void Insert(string cacheKey, object value, TimeSpan duration = default)
    {
        if (value == null)
            Remove(cacheKey);
        else
            cache.Set(cacheKey, value, DateTime.Now.Add(duration));
    }

    /// <summary>
    /// Remove a single item from cache based on cacheKey
    /// - If it does not exist, it does nothing
    /// - If context is not web, it does nothing
    /// </summary>
    public static void Remove(string cacheKey)
    {
        if (cacheKey.IsNot()) return;

        cache.Remove(cacheKey);
    }

    /// <summary>
    /// Clear everything found in cache
    /// </summary>
    public static void Clear()
    {
        var entries = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
        if (entries == null) return;

        var entriesCollection = entries.GetValue(cache) as ICollection;
        if (entriesCollection == null || entriesCollection.Count == 0) return;

        var keys = new List<string>();
        if (entriesCollection != null)
        {
            foreach (var item in entriesCollection)
            {
                var key = item.GetType().GetProperty("Key")?.GetValue(item);
                if (key != null)
                    keys.Add(key.ToString());
            }
        }
        lock (cacheLock)
        {
            foreach (var key in keys)
                cache.Remove(key);
        }
    }

    static bool SkipCache(bool skipForAuthenticatedUsers, bool skipForAdmins, Func<bool> skipFor)
    {
        if (skipForAuthenticatedUsers || skipForAdmins || skipFor != null)
        {
            if (skipForAuthenticatedUsers && IsCurrentUserAuthenticated())
                return true;

            if (skipForAdmins && IsCurrentUserAdmin())
                return true;

            if (skipFor != null && skipFor())
                return true;
        }

        return false;
    }

    static bool IsCurrentUserAuthenticated()
    {
        return Principal?.Identity?.IsAuthenticated == true;
    }

    static bool IsCurrentUserAdmin()
    {
        return Principal?.Identity?.IsAuthenticated == true && Principal.IsInAnyRole("Admin", "Administrators", "WebAdmins", "CmsAdmins");
    }
}
