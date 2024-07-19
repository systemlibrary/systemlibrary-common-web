using System;

namespace SystemLibrary.Common.Web;

partial class Client
{
    partial class ClientCached
    {
        const int MinimumLifetimeSeconds = 100;

        static DateTime LastDisposed;

        static void Dispose()
        {
            CleanDisposeQueue();
            LastDisposed = DateTime.Now.ToUniversalTime();
        }

        static void CleanDisposeQueue()
        {
            if (LastDisposed < DateTime.Now.AddSeconds(-10))
            {
                LastDisposed = DateTime.Now.ToUniversalTime();
                var disposedTime = DateTime.Now.AddSeconds(-ClientExpiresInSeconds - MinimumLifetimeSeconds);
                var keys = DisposeQueue.Keys;

                foreach (var key in keys)
                {
                    try
                    {
                        if (DisposeQueue.TryGetValue(key, out CacheModel clientQueuedCachedModel))
                        {
                            if (clientQueuedCachedModel.Expires < disposedTime)
                            {
                                DisposeQueue.TryRemove(key, out _);
                                clientQueuedCachedModel?.Dispose();
                            }
                        }
                    }
                    catch
                    {
                        // Note: Swalling due to if items are disposed twice within "same" cpu tick, multithreaded not tested, and I do not want to lock this
                    }
                }
            }
        }
    }
}