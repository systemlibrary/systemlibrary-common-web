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
            HttpBaseClient = new HttpBaseClientConfiguration();
            Log = new LogConfiguration();
            LogMessageBuilder = new LogMessageBuilderOptions();
            Client = new ClientConfiguration();
        }

        public bool Debug { get; set; } = false;

        public CacheConfiguration Cache { get; set; }
        public ClientConfiguration Client { get; set; }
        public HttpBaseClientConfiguration HttpBaseClient { get; set; }
        public LogConfiguration Log { get; set; }
        public LogMessageBuilderOptions LogMessageBuilder { get; set; }

        public class LogConfiguration
        {
            public LogLevel Level { get; set; } = LogLevel.Info;
        }

        public class HttpBaseClientConfiguration
        {
            public int TimeoutMilliseconds { get; set; } = 40000;

            public int RetryRequestTimeoutMs { get; set; } = 10000;
            public int CacheClientConnectionSeconds { get; set; } = 110;
        }

        public class ClientConfiguration
        {
            public int Timeout { get; set; } = Web.Client.DefaultTimeout;
            public int RetryTimeout { get; set; } = Web.Client.DefaultRetryTimeout;
            public bool IgnoreSslErrors { get; set; } = Web.Client.DefaultIgnoreSslErrors;
            public bool UseRetryPolicy { get; set; } = Web.Client.DefaultUseRetryPolicy;
            
            public int ClientCacheDuration = Web.Client.ClientCacheDuration;
            public bool ThrowOnUnsuccessful { get; set; } = Web.Client.DefaultThrowOnUnsuccessful;
            public bool UseCircuitBreakerPolicy { get; set; } = Web.Client.DefaultUseCircuitBreakerPolicy;
        }

        public class CacheConfiguration
        {
            public int DefaultDuration { get; set; } = 180;
            public int ContainerSizeLimit { get; set; } = 60000;
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
