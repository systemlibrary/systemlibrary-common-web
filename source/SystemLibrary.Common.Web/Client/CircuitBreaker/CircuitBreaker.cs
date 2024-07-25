using System;
using System.Collections.Concurrent;
using System.Net.Http;

using Polly;

namespace SystemLibrary.Common.Web;

internal static class CircuitBreaker
{
    static ConcurrentDictionary<string, IAsyncPolicy> Policies = new ConcurrentDictionary<string, IAsyncPolicy>();
  
    internal static IAsyncPolicy GetPolicy(string policyKey)
    {
        return Policies.GetOrAdd(policyKey, CreatePolicy());
    }

    static IAsyncPolicy CreatePolicy()
    {
        return Policy.Handle<HttpRequestException>()
            .CircuitBreakerAsync(25, TimeSpan.FromSeconds(7));
    }
}