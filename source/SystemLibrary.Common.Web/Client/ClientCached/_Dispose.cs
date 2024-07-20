using System;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web;

partial class Client
{
    partial class ClientCached
    {
        static object DisposeLock = new object();
        static DateTime Next;

        static void Dispose()
        {
            DisposeQueueInterval();
        }

        static void DisposeQueueInterval()
        {
            if (DateTime.Now < Next)
                return;

            var keys = DisposeQueue.Keys;

            if (keys.IsNot()) return;

            lock (DisposeLock)
            {
                if (DateTime.Now < Next)
                    return;

                Next = DateTime.Now.AddSeconds(15);
            }

            // 5 minutes after the last timeout could occur, HttpClient is disposed
            var dateTimeElapsed = DateTime.Now.AddMilliseconds(-TimeoutConfig - RetryTimeoutConfig - 300000);

            foreach (var key in keys)
            {
                if (DisposeQueue.TryGetValue(key, out CacheModel cached))
                {
                    try
                    {
                        if (cached.Expires < dateTimeElapsed)
                        {
                            DisposeQueue.TryRemove(key, out CacheModel removed);
                            removed?.Dispose();
                        }
                    }
                    catch
                    {
                        //Swallow
                    }
                }
            }
        }
    }
}