using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

using Microsoft.Extensions.Caching.Memory;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Web;

/// <summary>
/// Caching for web applications
///
/// Default duration is 180 seconds
/// 
/// Try using auto-generating cache keys, which differentiate caching down to user roles.
/// - Cache things per user, by userId/email? Create your own cacheKey
///
/// 'Skip' means that the item will not be fetched from cache
/// 
/// Skip options:
/// - skipForAuthenticatedUsers, false by default
/// - skipForAdmins, true by default
///     * User must be part of any following roles case sensitive: "Admin", "Admins", "Administrator", "Administrators", "WebAdmins", "CmsAdmins", "admin"
/// - skipFor, your own condition, must return True to skip
/// </summary>
/// <remarks>
/// Cache is limited to 240.000 items by default, divided by 4 containers, where any item added takes up 1 size
/// 
/// Each container contains up to 60.000 items, once reached 33% are removed ready to be GC'ed
/// 
/// Null is never added to cache
/// 
/// Overwrite default cache configurations in appSettings.json:
/// - cacheDuration: 180, minimum 1
/// - containerSizeLimit: 60000, minimum 100
/// 
/// Auto-generating cache key adds namespace, class, method, method-scoped variables of type bool, string, int, short, long, double, datetime, datetimeoffset and enum to the cache key, if used within the getItem method
/// - If a reference to class is used within the method, its public fields and properties of same types are also added
/// - IsAuthenticated is added to cache key, including ClaimsPrincipal roles if any
/// - Always adds prefix to avoid collisions
/// </remarks>
/// <example>
/// Configure the cache in appSettings.json 
/// <code class="language-csharp hljs">
/// {
///     "systemLibraryCommonWeb": {
///         "cache" { 
///             "defaultDuration": 210,
///             "containerSizeLimit": 60000
///         }
///     }
/// }
/// </code>
/// Use cache:
/// <code class="language-csharp hljs">
/// using SystemLibrary.Common.Web;
/// 
/// var cacheKey = "key";
/// var item = Cache.Get(cacheKey);
/// // null if not in cache
/// </code>
/// </example>
public static class Cache
{
    static IPrincipal _Principal;
    static IPrincipal Principal => _Principal ??= HttpContextInstance.Current?.User;

    static IMemoryCache[] cache;
    static int MaxCacheContainers = 4;

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
                    _DefaultDuration = 210;
                }
            }
            return _DefaultDuration;
        }
    }

    static Cache()
    {
        cache = new IMemoryCache[MaxCacheContainers];

        var containerSizeLimit = AppSettings.Current.SystemLibraryCommonWeb.Cache.ContainerSizeLimit;
        if (containerSizeLimit < 100)
            containerSizeLimit = 100;

        for (int i = 0; i < MaxCacheContainers; i++)
        {
            MemoryCacheOptions options = new MemoryCacheOptions();
            options.ExpirationScanFrequency = TimeSpan.FromSeconds(90 + Randomness.Int(30));
            options.SizeLimit = containerSizeLimit;
            options.CompactionPercentage = 0.33;
            cache[i] = new MemoryCache(options);
        }
    }

    /// <summary>
    /// Get item from Cache as T
    /// </summary>
    /// <remarks>
    /// CacheKey null or blank returns default without checking cache
    /// 
    /// Default duration is 180 seconds
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var cacheKey = "helloworld";
    /// var data = Cache.Get&lt;string&gt;(cacheKey);
    /// </code>
    /// </example>
    /// <returns>Returns item from cache or getItem, on exception returns default</returns>
    public static T Get<T>(string cacheKey)
    {
        if (cacheKey.IsNot()) return default;

        var cacheIndex = Math.Abs(cacheKey.GetHashCode() % 4);

        var cached = cache[cacheIndex].Get(cacheKey);

        return cached == null ? default : (T)cached;
    }

    /// <summary>
    /// Add item to cache for a duration
    /// 
    /// Null value is never added to cache
    /// </summary>
    /// <param name="cacheKey">CacheKey to set item as, if null or empty this does nothing</param>
    /// <param name="duration">Defaults to 180 seconds</param>
    public static void Set<T>(string cacheKey, T item, TimeSpan duration = default)
    {
        if (cacheKey.IsNot())
            return;

        var h = "hello";

        if (duration == default)
            duration = TimeSpan.FromSeconds(DefaultDuration);

        var cacheIndex = Math.Abs(cacheKey.GetHashCode() % 4);

        Insert(cacheIndex, cacheKey, item, duration);
    }

    /// <summary>
    /// Try get item from Cache as T
    /// 
    /// Null value is never added to cache
    /// 
    /// Logs exception if getItem() throws
    /// </summary>
    /// <remarks>
    /// Default duration is 180 seconds
    /// 
    /// 'Skip' means that the item will not be fetched from cache
    /// 
    /// Skip options:
    /// - skipForAuthenticatedUsers, false by default
    /// - skipForAdmins, true by default
    ///     * User must be part of any following roles case sensitive: "Admin", "Admins", "Administrator", "Administrators", "WebAdmins", "CmsAdmins", "admin"
    /// - skipFor, your own condition, must return True to skip
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var cacheKey = "key";
    /// 
    /// var data = Cache.TryGet&lt;string&gt;(cacheKey, () => throw new Exception("does not crash application"));
    /// 
    /// // Exception is logged through your ILogWriter implementation
    /// </code>
    /// </example>
    /// <returns>Returns item from cache or getItem, on exception returns default</returns>
    public static T TryGet<T>(string cacheKey, Func<T> getItem, TimeSpan duration = default, Func<T, bool> condition = null, bool skipForAuthenticatedUsers = false, bool skipForAdmins = true, Func<bool> skipFor = null)
    {
        try
        {
            return Get(getItem, cacheKey, duration, condition, skipForAuthenticatedUsers, skipForAdmins, skipFor);
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return default;
        }
    }

    /// <summary>
    /// Try get item from Cache as T using auto-generated cache key
    /// 
    /// Null value is never added to cache
    /// 
    /// Logs exception if getItem() throws
    /// </summary>
    /// <remarks>
    /// Default duration is 180 seconds
    /// 
    /// 'Skip' means that the item will not be fetched from cache
    /// 
    /// Skip options:
    /// - skipForAuthenticatedUsers, false by default
    /// - skipForAdmins, true by default
    ///     * User must be part of any following roles case sensitive: "Admin", "Admins", "Administrator", "Administrators", "WebAdmins", "CmsAdmins", "admin"
    /// - skipFor, your own condition, must return True to skip
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var data = Cache.TryGet&lt;string&gt;(() => throw new Exception("does not crash application"));
    /// 
    /// // Exception is logged through your ILogWriter implementation
    /// </code>
    /// </example>
    /// <returns>Returns item from cache or getItem, on exception returns default</returns>
    public static T TryGet<T>(Func<T> getItem, TimeSpan duration, Func<T, bool> condition = null, bool skipForAuthenticatedUsers = false, bool skipForAdmins = true, Func<bool> skipFor = null)
    {
        try
        {
            return Get(getItem, "", duration, condition, skipForAuthenticatedUsers, skipForAdmins, skipFor);
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return default;
        }
    }

    /// <summary>
    /// Try get item from Cache as T using auto-generated cache key
    /// 
    /// Null value is never added to cache
    /// 
    /// Logs exception if getItem() throws
    /// </summary>
    /// <remarks>
    /// Default duration is 180 seconds
    /// 
    /// 'Skip' means that the item will not be fetched from cache
    /// 
    /// Skip options:
    /// - skipForAuthenticatedUsers, false by default
    /// - skipForAdmins, true by default
    ///     * User must be part of any following roles case sensitive: "Admin", "Admins", "Administrator", "Administrators", "WebAdmins", "CmsAdmins", "admin"
    /// - skipFor, your own condition, must return True to skip
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var data = Cache.TryGet&lt;string&gt;(() => throw new Exception("does not crash application"));
    /// 
    /// // Exception is logged through your ILogWriter implementation
    /// </code>
    /// </example>
    /// <returns>Returns item from cache or getItem, on exception returns default</returns>
    public static T TryGet<T>(Func<T> getItem, string cacheKey = "", TimeSpan duration = default, Func<T, bool> condition = null, bool skipForAuthenticatedUsers = false, bool skipForAdmins = true, Func<bool> skipFor = null)
    {
        try
        {
            return Get(getItem, cacheKey, duration, condition, skipForAuthenticatedUsers, skipForAdmins, skipFor);
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return default;
        }
    }

    /// <summary>
    /// Get item from Cache as T
    /// 
    /// Null value is never added to cache
    /// </summary>
    /// <remarks>
    /// Throws exception if getItem can throw
    /// 
    /// Default duration is 180 seconds
    /// 
    /// 'Skip' means that the item will not be fetched from cache
    /// 
    /// Skip options:
    /// - skipForAuthenticatedUsers, false by default
    /// - skipForAdmins, true by default
    ///     * User must be part of any following roles case sensitive: "Admin", "Admins", "Administrator", "Administrators", "WebAdmins", "CmsAdmins", "admin"
    /// - skipFor, your own condition, must return True to skip
    /// </remarks>
    /// <code class="language-csharp hljs">
    /// class CarService
    /// {
    ///     public string GetCars() 
    ///     {
    ///         var cacheKey = "helloworld";
    ///         
    ///         return Cache.Get&lt;string&gt;(cacheKey, () => {
    ///             return HttpBaseClient.Get&lt;string&gt;("https://systemlibrary.com/api/cars?top=1");
    ///         },
    ///         TimeSpan.FromSeconds(5));
    ///     }
    /// }
    /// </code>
    /// <returns>Returns item from cache or getItem</returns>
    public static T Get<T>(string cacheKey, Func<T> getItem, TimeSpan duration = default, Func<T, bool> condition = null, bool skipForAuthenticatedUsers = false, bool skipForAdmins = true, Func<bool> skipFor = null)
    {
        return Get(getItem, cacheKey, duration, condition, skipForAuthenticatedUsers, skipForAdmins, skipFor);
    }

    /// <summary>
    /// Get item from Cache as T using auto-generated cache key
    /// 
    /// Null value is never added to cache
    /// </summary>
    /// <remarks>
    /// Throws exception if getItem can throw
    /// 
    /// Default duration is 180 seconds
    /// 
    /// 'Skip' means that the item will not be fetched from cache
    /// 
    /// Skip options:
    /// - skipForAuthenticatedUsers, false by default
    /// - skipForAdmins, true by default
    ///     * User must be part of any following roles case sensitive: "Admin", "Admins", "Administrator", "Administrators", "WebAdmins", "CmsAdmins", "admin"
    /// - skipFor, your own condition, must return True to skip
    /// </remarks>
    /// <code class="language-csharp hljs">
    /// class CarService
    /// {
    ///     public string GetCars() 
    ///     {
    ///         var cacheKey = "helloworld";
    ///         
    ///         return Cache.Get&lt;string&gt;(cacheKey, () => {
    ///             return HttpBaseClient.Get&lt;string&gt;("https://systemlibrary.com/api/cars?top=1");
    ///         },
    ///         TimeSpan.FromSeconds(5));
    ///     }
    /// }
    /// </code>
    /// <returns>Returns item from cache or getItem</returns>
    public static T Get<T>(Func<T> getItem, TimeSpan duration, Func<T, bool> condition = null, bool skipForAuthenticatedUsers = false, bool skipForAdmins = true, Func<bool> skipFor = null)
    {
        return Get(getItem, "", duration, condition, skipForAuthenticatedUsers, skipForAdmins, skipFor);
    }

    /// <summary>
    /// Get item from Cache as T using auto-generated cache key
    /// 
    /// Null value is never added to cache
    /// </summary>
    /// <remarks>
    /// Throws exception if getItem can throw
    /// 
    /// Default duration is 180 seconds
    /// 
    /// 'Skip' means that the item will not be fetched from cache
    /// 
    /// Skip options:
    /// - skipForAuthenticatedUsers, false by default
    /// - skipForAdmins, true by default
    ///     * User must be part of any following roles case sensitive: "Admin", "Admins", "Administrator", "Administrators", "WebAdmins", "CmsAdmins", "admin"
    /// - skipFor, your own condition, must return True to skip
    /// </remarks>
    /// <param name="cacheKey">"" to use auto-generating of cacheKey, null to always skip cache</param>
    /// <param name="condition">Add to cache only if condition is met, for instance: data != null</param>
    /// <param name="skipForAuthenticatedUsers">Skip cache for any user that is authenticated, but is not part of any of the admin roles: Admins, Administrators, WebAdmins, CmsAdmins</param>
    /// <param name="skipForAdmins">Skip cache for current principal that is authenticated and is part of either of the roles: Admins, Administrators, WebAdmins, CmsAdmins</param>
    /// <param name="skipFor">Implement your own logic for when to skip cache, let it return true on your conditions to avoid caching</param>
    /// <example>
    /// Simplest example:
    /// <code class="language-csharp hljs">
    /// var data = Cache.Get(() => {
    ///     return "hello world";
    /// });
    /// 
    /// //'data' is now 'hello world', if called multiple times within the default cache duration of 180 seconds, "hello world" is returned from the cache for all non-admin users
    /// </code>
    /// 
    /// Simplest example with cacheKey:
    /// <code class="language-csharp hljs">
    /// var cacheKey = "hello-world-key";
    /// var data = Cache.Get(() => {
    ///     return "hello world";
    /// },
    /// cacheKey: cacheKey);
    /// 
    /// //'data' is now 'hello world', if called multiple times within the default cache duration of 180 seconds, "hello world" is returned from the cache for all non-admin users
    /// </code>
    /// 
    /// Example with multiple options passed, and a condition that always fails:
    /// <code class="language-csharp hljs">
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
    /// // so in this scenario - "hello world" is never added to cache, and our function that returns "hello world" is always invoked
    /// </code>
    /// 
    /// Example without a cache key
    /// <code class="language-csharp hljs">
    /// class CarService
    /// {
    ///     public string GetCars() 
    ///     {
    ///         return Cache.Get&lt;string&gt;(() => {
    ///             return HttpBaseClient.Get&lt;string&gt;("https://systemlibrary.com/api/cars?top=1");
    ///         },
    ///         skipForAdmins: false);
    ///     }
    /// }
    /// // This caches top 1 cars for every user, even admins, as we set 'skipForAdmins' to False
    /// </code>
    /// 
    /// Example without a cache key and with 'external' variables
    /// <code class="language-csharp hljs">
    /// class CarService
    /// {
    ///     public string GetCars(int top = 10) 
    ///     {
    ///         var url = "https://systemlibrary.com/api/cars";
    ///         var urlQueryValue = "?filter=none";
    ///         
    ///         return Cache.Get&lt;string&gt;(() => {
    ///             return HttpBaseClient.Get&lt;string&gt;(url + urlQueryValue + " top=" + top);
    ///         });
    ///     }
    /// }
    /// 
    /// // Returns top 10 cars from the API, and adds result to cache (assumes not null) for a duration of 180 seconds by default
    /// // For simplicity, pretend an auto cache key looks like this: sysLib.web.CarService.GetCars_top=10_systemlibrary.com/api/cars_?filter=none_IsAuthenticated=false
    /// 
    /// // Note: cache key is created with the outside variable "top", it is ".ToString'd", works on many types: bool, datetime, string, and simple POCO's with 1 depth level of properties/fields, not "class inside class" is not supported
    /// // Note: cache key for wether or not user is logged in is always appended so it always varies on "IsAuthenticated"
    /// </code>
    /// </example>
    /// <returns>Returns item from cache or getItem</returns>
    public static T Get<T>(Func<T> getItem, string cacheKey = "", TimeSpan duration = default, Func<T, bool> condition = null, bool skipForAuthenticatedUsers = false, bool skipForAdmins = true, Func<bool> skipFor = null)
    {
        if (cacheKey == null)
            return getItem();

        if (SkipCache(skipForAuthenticatedUsers, skipForAdmins, skipFor))
        {
            Debug.Log("Skipping cache for cachey key: " + cacheKey);

            return getItem();
        }

        if (duration == default)
            duration = TimeSpan.FromSeconds(DefaultDuration);

        if (cacheKey == "")
            cacheKey = CreateCacheKey(getItem, condition);

        var cacheIndex = Math.Abs(cacheKey.GetHashCode() % 4);

        var cached = cache[cacheIndex].Get(cacheKey);

        if (cached != null)
        {
            Debug.Log("Item was cached with key " + cacheKey);

            return (T)cached;
        }
        else
        {
            Debug.Log("Item was not cached with key " + cacheKey);
        }

        cached = getItem();

        if (cached != null && (condition == null || condition((T)cached)))
        {
            Debug.Log("Item met conditions, added to Cache, key: " + cacheKey);

            Insert(cacheIndex, cacheKey, cached, duration);
        }

        return (T)cached;
    }

    /// <summary>
    /// Create a 'lock' to part of a function, to run it only once within the duration
    /// 
    /// Default lock duration is 60 seconds
    /// 
    /// Useful to execute code only once within the time frame per app instance, not bombarding log for instance
    /// </summary>
    /// <remarks>
    /// Uses the stack frame to read current namespace and method as cache key, so max 1 invocation per function scope, else you must fill out the cacheLock parameter too
    /// - in the future it might support multiple...
    /// </remarks>
    /// <param name="lockKey">Append data to the lock key, if multiple locks resides inside the same method scope</param>
    /// <example>
    /// <code class="language-csharp hljs">
    /// if(Cache.Lock("send-email", TimeSpan.FromSeconds(60)) 
    /// {
    ///     new Email(...).Send(); // Pseudo code
    ///     // Example: invoking this code 66 times, one time per second, where first invocation is one second from "now", will send two emails: one at second 1, and another at second 61
    /// }
    /// </code>
    /// </example>
    /// <returns>Returns true if the key do not exist or has expired else returns false</returns>
    public static bool Lock(TimeSpan duration = default, string lockKey = null)
    {
        if (duration == default)
            duration = TimeSpan.FromSeconds(60);

        try
        {
            var callee = new StackFrame(1).GetMethod();

            var cacheKey = nameof(SystemLibrary) + nameof(Cache) + nameof(Lock) + callee.DeclaringType?.Namespace + callee.DeclaringType?.Name + callee.Name + callee.IsStatic + callee.IsPublic + duration + lockKey;

            var cacheIndex = Math.Abs(cacheKey.GetHashCode() % 4);

            var exists = cache[cacheIndex].Get<bool>(cacheKey);

            if (exists) return false;

            Insert(cacheIndex, cacheKey, true, duration);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Removes item from cache or does nothing if item do not exist in cache
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var cacheKey = "hello world";
    /// Cache.Remove(cacheKey);
    /// </code>
    /// </example>
    public static void Remove(string cacheKey)
    {
        if (cacheKey.IsNot()) return;

        var cacheIndex = Math.Abs(cacheKey.GetHashCode() % 4);

        cache[cacheIndex].Remove(cacheKey);
    }

    /// <summary>
    /// Clear all entries found, which was set through this Cache class
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// Cache.Clear();
    /// </code>
    /// </example>
    public static void Clear()
    {
        var entries = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
        if (entries == null) return;

        for (int i = 0; i < MaxCacheContainers; i++)
        {
            var entriesCollection = entries.GetValue(cache[i]) as ICollection;
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

            foreach (var key in keys)
                cache[i].Remove(key);

            keys.Clear();
            keys = null;
        }
    }

    static void Insert(int cacheIndex, string cacheKey, object item, TimeSpan duration)
    {
        if (item == null)
            Remove(cacheKey);
        else
            cache[cacheIndex].Set(cacheKey, item, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.Add(duration),
                Size = 1,
            });
    }

    static string CreateCacheKey<T>(Func<T> getItem, Func<T, bool> condition)
    {
        var key = new StringBuilder("common.web.cache", capacity: 383);

        var getItemMethod = getItem.Method;

        key.Append(getItemMethod.Name);
        key.Append(getItemMethod.DeclaringType?.FullName ?? "");
        key.Append(getItemMethod.ReturnType?.FullName ?? "");

        var target = getItem.Target;
        if (target != null)
        {
            void AppendString(object value, Type valueType)
            {
                if (value is string text)
                {
                    if (text.Length > 96)
                    {
                        key.Append(text.Length + text.MaxLength(96) + text[^5]);
                    }
                    else
                    {
                        key.Append(text);
                    }
                }
                else if (value is StringBuilder sb)
                {
                    if (sb.Length > 96)
                    {
                        var temp = sb.ToString();
                        key.Append(sb.Length + temp.MaxLength(96) + temp[^5]);
                        temp = null;
                    }
                    else
                    {
                        key.Append(sb.ToString());
                    }
                }
                else if (value is Guid g)
                {
                    key.Append(g.ToString("N"));
                }
                else if (value is DateTime dt)
                {
                    key.Append(dt.ToString("yyyyMMddHHmmss"));
                }
                else if (IsToStringable(valueType))
                {
                    key.Append(value.ToString());
                }
                else
                {
                    Debug.Log(valueType.Name + " not stringable type: " + value);
                }
            }

            void AppendClass(object value, Type valueType)
            {
                var valueProperties = Dictionaries.GenerateCacheKeyValueTypeProperties.Cache(valueType, () =>
                {
                    return valueType.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Instance);
                });

                var valueFields = Dictionaries.GenerateCacheKeyValueTypeFields.Cache(valueType, () =>
                {
                    return valueType.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance);
                });

                if (valueProperties?.Length > 0)
                {
                    foreach (var pi in valueProperties)
                    {
                        if (!IsToStringable(pi.PropertyType)) continue;

                        key.Append(pi.Name);

                        try
                        {
                            MethodInfo getMethod = pi.GetGetMethod();

                            object piValue = null;
                            if (getMethod.IsStatic)
                            {
                                piValue = pi.GetValue(null);
                            }
                            else
                            {
                                piValue = pi.GetValue(value);
                            }

                            if (piValue != null)
                                AppendString(piValue, piValue.GetType());
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                }

                if (valueFields?.Length > 0)
                {
                    foreach (var fi in valueFields)
                    {
                        if (!IsToStringable(fi.FieldType)) continue;

                        key.Append(fi.Name);

                        try
                        {
                            object fiValue = null;
                            if (fi.IsStatic)
                            {
                                fiValue = fi.GetValue(null);
                            }
                            else
                            {
                                fiValue = fi.GetValue(value);
                            }
                            if (fiValue != null)
                                AppendString(fiValue, fiValue.GetType());
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                }
            }

            void AppendCollection(ICollection collection)
            {
                key.Append(collection.Count);

                foreach (var value in collection)
                {
                    AppendValue(value);
                }
            }

            void AppendValue(object value)
            {
                if (key.Length > 2048) return;

                if (value == null) return;

                var valueType = value.GetType();

                if (IsToStringable(valueType))
                    AppendString(value, valueType);
                else if (value is ICollection collection)
                    AppendCollection(collection);
                else if (valueType.IsClass)
                    AppendClass(value, valueType);
                else
                {
                    Debug.Log(valueType.Name + " not appendable");
                }
            }

            void AppendFieldArgument(FieldInfo field)
            {
                if (key.Length > 2048) return;

                if (field == null) return;

                var type = field.FieldType;

                if (!IsTypeAutoCacheKeyType(type))
                {
                    Debug.Log("Field type not part of cache key " + type.Name);

                    return;
                }

                key.Append(field.Name.MaxLength(20));

                try
                {
                    object value;
                    if (field.IsStatic)
                    {
                        value = field.GetValue(null);
                    }
                    else
                    {
                        value = field.GetValue(target);
                    }

                    AppendValue(value);
                }
                catch
                {
                    // Swallow
                }
            }

            var type = target.GetType();

            var fields = Dictionaries.GenerateCacheKeyFields.Cache(type, () =>
            {
                return type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            });

            if (fields?.Length > 0)
            {
                foreach (var field in fields)
                {
                    AppendFieldArgument(field);
                }
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

                if (claimsPrincipal?.Claims != null)
                {
                    var roles = claimsPrincipal.Claims
                        .Where(c => c.Type == claimsIdentity.RoleClaimType || c.Type == "role" || c.Type == "Role")
                        .Select(x => x.Value);

                    if (roles != null)
                        key.Append(string.Join("", roles));
                }
            }
        }

        return key.ToString();
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
        return Principal?.Identity?.IsAuthenticated == true && Principal.IsInAnyRole("Admin", "Admins", "Administrator", "Administrators", "WebAdmins", "CmsAdmins", "admin", "administrators");
    }

    static bool IsTypeAutoCacheKeyType(Type type)
    {
        if (type.IsEnum) return true;

        if (type.Inherits(SystemType.ICollectionType)) return true;

        if (type == SystemType.UriType) return true;

        if (type.IsKeyValuePair()) return true;

        if (type.IsClass && !type.IsGenericType) return true;

        return Array.IndexOf(AutoCacheKeyStringableTypes, type) >= 0;
    }

    static bool IsToStringable(Type type)
    {
        if (type.IsEnum) return true;

        if (type.IsKeyValuePair()) return true;

        return Array.IndexOf(AutoCacheKeyStringableTypes, type) >= 0;
    }

    static Type AutoCacheKeyICollection = typeof(ICollection);

    static Type[] AutoCacheKeyStringableTypes = {
            typeof(string),
            typeof(StringBuilder),
            typeof(bool),
            typeof(int),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(float),
            typeof(double),
            typeof(Enum),
            typeof(short),
            typeof(long),
            typeof(decimal),
            typeof(uint),
            typeof(Uri),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(KeyValuePair<,>)
    };

}
