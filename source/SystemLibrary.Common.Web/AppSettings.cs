using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web;

internal class AppSettings : Config<AppSettings>
{
    public AppSettings()
    {
        SystemLibraryCommonWeb = new PackageConfig();
    }

    public class PackageConfig
    {
        public PackageConfig()
        {
            Cache = new CacheConfiguration();
            Log = new LogConfiguration();
            LogMessageBuilder = new LogMessageBuilderOptions();
            Client = new ClientConfiguration();
        }

        public bool Debug { get; set; } = false;

        public CacheConfiguration Cache { get; set; }
        public ClientConfiguration Client { get; set; }
        public LogConfiguration Log { get; set; }
        public LogMessageBuilderOptions LogMessageBuilder { get; set; }

        public class LogConfiguration
        {
            public LogLevel Level { get; set; } = LogLevel.Info;
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

        public class LogMessageBuilderOptions
        {
            public bool AppendLoggedInState { get; set; } = true;
            public bool AppendIp { get; set; } = false;

            public bool AppendPath { get; set; } = true;

            public bool AppendBrowser { get; set; } = false;
            public bool AppendCorrelationId { get; set; } = true;
            public bool AppendCookieInfo { get; set; } = false;
            public string Format { get; set; }  // json | null
        }
    }

    public PackageConfig SystemLibraryCommonWeb { get; set; }
}
