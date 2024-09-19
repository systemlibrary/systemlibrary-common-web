namespace SystemLibrary.Common.Web;

internal class PackageConfig
{
    public bool Debug { get; set; } = false;
    public CacheConfiguration Cache { get; set; }
    public ClientConfiguration Client { get; set; }
    public LogConfiguration Log { get; set; }

    public PackageConfig()
    {
        Cache = new CacheConfiguration();
        Log = new LogConfiguration();
        Client = new ClientConfiguration();
    }

    public class LogConfiguration
    {
        public LogLevel? Level { get; set; }

        public bool AppendLoggedInState { get; set; } = true;
        public bool AppendIp { get; set; } = false;

        public bool AppendPath { get; set; } = true;
        public bool AppendBrowser { get; set; } = false;
        public bool AppendCorrelationId { get; set; } = true;
        public bool AppendCookieInfo { get; set; } = false;
        public string Format { get; set; }  // json | null
    }

    public class ClientConfiguration
    {
        public int Timeout { get; set; } = Web.Client.DefaultTimeout;
        public int RetryTimeout { get; set; } = Web.Client.DefaultRetryTimeout;
        public bool IgnoreSslErrors { get; set; } = Web.Client.DefaultIgnoreSslErrors;
        public bool UseRetryPolicy { get; set; } = Web.Client.DefaultUseRetryPolicy;
        public int ClientCacheDuration { get; set; } = Web.Client.DefaultClientCacheDuration;
        public bool ThrowOnUnsuccessful { get; set; } = Web.Client.DefaultThrowOnUnsuccessful;
        public bool UseRequestBreakerPolicy { get; set; } = Web.Client.DefaultUseRequestBreakerPolicy;
    }

    public class CacheConfiguration
    {
        public int Duration { get; set; } = Web.Cache.DefaultDuration;
        public int ContainerSizeLimit { get; set; } = Web.Cache.DefaultContainerSizeLimit;
        public int FallbackDuration { get; set; } = Web.Cache.DefaultFallbackDuration;
    }

}

