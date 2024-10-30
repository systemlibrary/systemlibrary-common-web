using System;

using Prometheus;

namespace SystemLibrary.Common.Web;

partial class Client
{
    static readonly Counter ClientSuccessCounter = Metrics.CreateCounter(
        "client_requests_success_total",
        "Counts successful requests",
        new CounterConfiguration
        {
            LabelNames = new[] { "uri" }
        });

    static readonly Counter ClientRetrySuccessCounter = Metrics.CreateCounter(
        "client_requests_retry_success_total",
        "Counts successful retries",
        new CounterConfiguration
        {
            LabelNames = new[] { "uri" }
        });

    static readonly Counter ClientFailureCounter = Metrics.CreateCounter(
        "client_requests_failure_total",
        "Counts failed requests",
        new CounterConfiguration
        {
            LabelNames = new[] { "uri" }
        });

    static readonly Counter ClientCircuitBreakCounter = Metrics.CreateCounter(
        "client_circuit_break_total",
        "Counts circuit breaking events",
        new CounterConfiguration
        {
            LabelNames = new[] { "uri" }
        });


    string GetUriLabel(string typeName, Uri uri)
    {
        if (typeName == "Client")
        {
            typeName = uri.Host.ToLowerInvariant();
        }

        if (uri.AbsolutePath.Length <= 1)
        {
            return typeName;
        }

        var path = uri.AbsolutePath.ToLowerInvariant();

        if (path.EndsWith("/"))
            path = path.Substring(0, path.Length - 1);

        var parts = path.Split('/');

        // Append the first up to 2 segments to the normalized URI
        for (int i = 0; i < Math.Min(parts.Length, 2); i++)
        {
            if (parts[i].IsNot()) continue;

            typeName += $"/{parts[i]}";
        }
        return typeName;

    }
}
