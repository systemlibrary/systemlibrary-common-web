using System;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    partial class Client
    {
        const int MinimumLifetimeSeconds = 120;

        static void Dispose()
        {
            CleanDisposeQueue();
        }

        static void CleanDisposeQueue()
        {
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