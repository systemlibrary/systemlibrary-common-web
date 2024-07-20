using System;
using System.Collections.Concurrent;
using System.Net.Http;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web;

partial class Client
{
    //TODO: https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient/69826649#69826649
    //A way to get "current percentage of downloaded file/stream/uploading..."
    partial class ClientCached
    {
        static int _ClientCacheDuration = -1;
        static int ClientCacheDuration
        {
            get
            {
                if (_ClientCacheDuration == -1)
                {
                    _ClientCacheDuration = AppSettings.Current.SystemLibraryCommonWeb.Client.ClientCacheDuration;

                    // Set to 0 to avoid looking up the config again, a 0 re-created httpclient every time
                    if (_ClientCacheDuration < 0)
                        _ClientCacheDuration = 0;
                }
                return _ClientCacheDuration;
            }
        }

        static ConcurrentDictionary<string, CacheModel> Cache;
        static ConcurrentDictionary<string, CacheModel> DisposeQueue;

        static ClientCached()
        {
            Cache = new ConcurrentDictionary<string, CacheModel>();
            DisposeQueue = new ConcurrentDictionary<string, CacheModel>();
        }

        internal static HttpClient GetClient(string url, int timeoutMilliseconds, bool retryOnTransientErrors = false, bool forceNewClient = false, bool ignoreSslErrors = false)
        {
            var uri = new Uri(url);

            var key = $"{nameof(Client)}{nameof(GetClient)}{uri.Scheme}{uri.Authority}{uri.Port}#{timeoutMilliseconds}#{retryOnTransientErrors}#{ignoreSslErrors}";

            if (forceNewClient)
            {
                RemoveFromCache(key);
            }
            else if (Cache.TryGetValue(key, out CacheModel cached))
            {
                if (HasExpired(cached))
                    RemoveFromCache(key);
                else
                    return cached.CachedClient;
            }

            return New(key, timeoutMilliseconds, retryOnTransientErrors, ignoreSslErrors);
        }

        static HttpClient New(string key, int timeoutMilliseconds, bool retryOnTransientErrors, bool ignoreSslErrors)
        {
            var socketsHandler = new SocketsHttpHandler
            {
                // Each http client's connection is reused for 290 seconds (slightly less than 5 min)
                // once reached, the connection is reestablished no matter what
                PooledConnectionLifetime = TimeSpan.FromSeconds(290),
                // If a connection is idle for 55 seconds (slightly less than 1 min) it is removed
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(55),
                ConnectTimeout = TimeSpan.FromSeconds(30),
                AllowAutoRedirect = true,
            };

            if (ignoreSslErrors)
            {
                socketsHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions()
                {
                    RemoteCertificateValidationCallback = (message, cert, chain, errors) =>
                    {
                        if (errors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors ||
                            errors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch ||
                            errors == System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable)
                        {
                            Log.Warning("Client: SslPolicy error occured, " + errors + ". Usually invalid or expired. IgnoreSslErrors is set to 'true' so continuing...");
                        }
                        return true;
                    }
                };
            }

            var timeoutHandler = new TimeoutHandler(timeoutMilliseconds, socketsHandler);

            var client = new HttpClient(timeoutHandler, disposeHandler: true);

            if (ClientCacheDuration > 0)
            {
                var httpClientCacheModel = new CacheModel()
                {
                    CachedClient = client,
                    Expires = DateTime.Now.AddMilliseconds(ClientCacheDuration)
                };
                Cache.TryAdd(key, httpClientCacheModel);
            }

            return client;
        }

        static bool HasExpired(CacheModel httpClientCached)
        {
            return httpClientCached?.CachedClient == null || httpClientCached.Expires < DateTime.Now;
        }

        static void RemoveFromCache(string key)
        {
            if (Cache.TryRemove(key, out CacheModel httpClientCached))
            {
                Dispose();

                if (httpClientCached != null)
                {
                    DisposeQueue.TryAdd(key + DateTime.Now.ToString("HH:mm:ss.fffff") + "#" + Randomness.Int() + Randomness.String(6), httpClientCached);
                }
            }
        }
    }
}