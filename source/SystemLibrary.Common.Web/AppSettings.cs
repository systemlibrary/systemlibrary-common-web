using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web;

internal class AppSettings : Config<AppSettings>
{
    public AppSettings()
    {
        SystemLibraryCommonWeb = new Configuration();
    }

    public class Configuration
    {
        public Configuration()
        {
            Cache = new CacheConfiguration();
            HttpBaseClient = new HttpBaseClientConfiguration();
            Log = new LogConfiguration();
            LogMessageBuilder = new LogMessageBuilderOptions();
        }
        
        public CacheConfiguration Cache { get; set; }
        public HttpBaseClientConfiguration HttpBaseClient { get; set; }
        public LogConfiguration Log { get; set; }
        public LogMessageBuilderOptions LogMessageBuilder { get; set; }

        public class LogConfiguration
        {
            public bool IsEnabled { get; set; } = true;
            public LogLevel Level { get; set; } = LogLevel.Info;
        }

        public class HttpBaseClientConfiguration
        {
            public int TimeoutMilliseconds { get; set; } = 60000;

            public int RetryRequestTimeoutSeconds { get; set; } = 10;
            public int CacheClientConnectionSeconds { get; set; } = 120;
        }

        public class CacheConfiguration
        {
            public int DefaultDuration { get; set; } = 180;
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

    public Configuration SystemLibraryCommonWeb { get; set; }
}
