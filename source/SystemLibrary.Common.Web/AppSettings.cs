using SystemLibrary.Common.Net;

//Small note to self: IViewComponentSelector exist, which can grab html elements by Id? or DomQuerySelector? So ... frontend tests much?

namespace SystemLibrary.Common.Web;

/// <summary>
/// Override default configurations in 'SystemLibrary.Common.Web' by adding 'systemLibraryCommonWeb' object to 'appSettings.json'
/// </summary>
/// <example>
/// 'appSettings.json'
/// <code class="language-csharp hljs">
/// {
///     ...,
///     "systemLibraryCommonWeb": {
///         "httpBaseClient": {
///             "timeoutMilliseconds": 60000,
///             "retryRequestTimeoutSeconds": 10,
///             "cacheClientConnectionSeconds": 300
///         }
///     },
///     ...
/// }
/// </code>
/// </example>
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
            HttpBaseClient = new HttpBaseClientConfiguration();
            Cache = new CacheConfiguration();
            LogMessageBuilder = new LogMessageBuilderOptions();
            Log = new LogConfiguration();
        }

        public HttpBaseClientConfiguration HttpBaseClient { get; set; }
        public CacheConfiguration Cache { get; set; }
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
            public int CacheClientConnectionSeconds { get; set; } = 300;
        }

        public class CacheConfiguration
        {
            public int DefaultDuration { get; set; } = 180;
        }

        public class LogMessageBuilderOptions
        {
            public bool AppendLoggedInState { get; set; } = true;
            public bool AppendCurrentPage { get; set; } = true;
            public bool AppendCurrentUrl { get; set; } = true;
            public bool AppendIp { get; set; } = false;
            public bool AppendBrowser { get; set; } = false;
            public bool AppendCookieInfo { get; set; } = false;
        }
    }

    public Configuration SystemLibraryCommonWeb { get; set; }
}
