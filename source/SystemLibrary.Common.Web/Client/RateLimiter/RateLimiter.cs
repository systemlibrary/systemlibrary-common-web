using System;
using System.Collections.Concurrent;

using Polly;

namespace SystemLibrary.Common.Web;

internal static class RateLimiter
{
    static ConcurrentDictionary<string, IAsyncPolicy> Policies = new ConcurrentDictionary<string, IAsyncPolicy>();

    internal static IAsyncPolicy GetPolicy(string policyKey)
    {
        return Policies.GetOrAdd(policyKey, CreatePolicy());
    }

    static IAsyncPolicy CreatePolicy()
    {
        return Policy.RateLimitAsync(
            1000,
            TimeSpan.FromMinutes(1));
    }
}
