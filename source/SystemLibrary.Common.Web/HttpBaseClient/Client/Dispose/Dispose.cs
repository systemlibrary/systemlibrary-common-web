using System;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        partial class Client
        {
            static void Dispose()
            {
                CleanDisposeQueue();
            }

            static void CleanDisposeQueue()
            {
                var disposedTime = DateTime.Now.AddSeconds(-ClientExpiresInSeconds);
                var keys = DisposeQueue.Keys;
                foreach (var key in keys)
                {
                    try
                    {
                        if (DisposeQueue.TryGetValue(key, out CacheModel queueCached))
                        {
                            if (queueCached?.HttpClientCached != null && queueCached.Expires < disposedTime)
                            {
                                DisposeQueue.TryRemove(key, out _);
                                queueCached?.Dispose();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}