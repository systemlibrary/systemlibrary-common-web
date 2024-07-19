using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        //TODO: https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient/69826649#69826649
        //A way to get "current percentage of downloaded file/stream/uploading..."
        partial class Client
        {
            static int _ClientExpiresInSeconds = -1;
            static int ClientExpiresInSeconds => _ClientExpiresInSeconds > -1 ? _ClientExpiresInSeconds :
                (_ClientExpiresInSeconds = AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.CacheClientConnectionSeconds);

            static ConcurrentDictionary<string, CacheModel> Cache;
            static ConcurrentDictionary<string, CacheModel> DisposeQueue;

            static Client()
            {
                Cache = new ConcurrentDictionary<string, CacheModel>();
                DisposeQueue = new ConcurrentDictionary<string, CacheModel>();
            }

            internal static HttpClient GetClient(string url, int timeoutMilliseconds, bool retryOnTransientErrors = false, bool forceNewClient = false, bool ignoreSslErrors = false)
            {
                var uri = new Uri(url);

                var key = nameof(HttpBaseClient) + nameof(GetClient) + uri.Scheme + uri.Authority + uri.Port + "#" + timeoutMilliseconds + "#" + retryOnTransientErrors + "#" + ignoreSslErrors;

                if (forceNewClient)
                {
                    RemoveFromCache(key);
                }
                else if (Cache.TryGetValue(key, out CacheModel cached))
                {
                    if (HasExpired(cached))
                        RemoveFromCache(key);
                    else
                        return cached.HttpClientCached;
                }

                return New(key, timeoutMilliseconds, retryOnTransientErrors, ignoreSslErrors);
            }

            static HttpClient New(string key, int timeoutMilliseconds, bool retryOnTransientErrors, bool ignoreSslErrors)
            {
                var sslHandler = new SslIgnoreHandler(ignoreSslErrors);

                var retryHandler = new RetryHandler(retryOnTransientErrors, sslHandler);

                var timeoutHandler = new TimeoutHandler(timeoutMilliseconds, retryHandler);

                var httpClientCacheModel = new CacheModel()
                {
                    HttpClientCached = new HttpClient(timeoutHandler, disposeHandler: true),
                    Expires = DateTime.Now.AddSeconds(ClientExpiresInSeconds)
                };

                if (ClientExpiresInSeconds > 0)
                {
                    Cache.TryAdd(key, httpClientCacheModel);
                }
                return httpClientCacheModel.HttpClientCached;
            }

            static bool HasExpired(CacheModel httpClientCached)
            {
                return httpClientCached?.HttpClientCached == null || httpClientCached.Expires < DateTime.Now;
            }

            static void RemoveFromCache(string key)
            {
                Cache.TryRemove(key, out CacheModel httpClientCached);

                Dispose();

                if (httpClientCached != null)
                    DisposeQueue.TryAdd(key + DateTime.Now.ToString("hh:mm:ss.fffff") + "#" + new Random().Next(1, 999999), httpClientCached);
            }
        }
    }
}