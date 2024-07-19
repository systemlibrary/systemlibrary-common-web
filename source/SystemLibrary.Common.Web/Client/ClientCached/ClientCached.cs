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
        static int _ClientExpiresInSeconds = -1;
        static int ClientExpiresInSeconds => _ClientExpiresInSeconds > -1 ? _ClientExpiresInSeconds :
            (_ClientExpiresInSeconds = AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.CacheClientConnectionSeconds);

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

            var key = nameof(Client) + nameof(GetClient) + uri.Scheme + uri.Authority + uri.Port + "#" + timeoutMilliseconds + "#" + retryOnTransientErrors + "#" + ignoreSslErrors;

            if (forceNewClient)
            {
                Debug.Log("Forcing a new client towards " + url);

                RemoveFromCache(key);
            }
            else if (Cache.TryGetValue(key, out CacheModel cached))
            {
                Debug.Log("Returning client from Cache, expired? " + HasExpired(cached));

                if (HasExpired(cached))
                    RemoveFromCache(key);
                else
                    return cached.CachedClient;
            }

            Debug.Log("Creating a new client to " + key);

            return New(key, timeoutMilliseconds, retryOnTransientErrors, ignoreSslErrors);
        }

        static HttpClient New(string key, int timeoutMilliseconds, bool retryOnTransientErrors, bool ignoreSslErrors)
        {
            var sslHandler = new SslIgnoreHandler(ignoreSslErrors);

            var timeoutHandler = new TimeoutHandler(timeoutMilliseconds, sslHandler);

            var httpClientCacheModel = new CacheModel()
            {
                CachedClient = new HttpClient(timeoutHandler, disposeHandler: true),
                Expires = DateTime.Now.AddSeconds(ClientExpiresInSeconds)
            };

            if (ClientExpiresInSeconds > 0)
            {
                Cache.TryAdd(key, httpClientCacheModel);
            }
            return httpClientCacheModel.CachedClient;
        }

        static bool HasExpired(CacheModel httpClientCached)
        {
            return httpClientCached?.CachedClient == null || httpClientCached.Expires < DateTime.Now;
        }

        static void RemoveFromCache(string key)
        {
            Cache.TryRemove(key, out CacheModel httpClientCached);

            Dispose();

            if (httpClientCached != null)
                DisposeQueue.TryAdd(key + DateTime.Now.ToString("HH:mm:ss.fffff") + "#" + Randomness.Int() + Randomness.String(6), httpClientCached);
        }
    }
}