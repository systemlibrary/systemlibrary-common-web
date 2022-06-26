using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web
{
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
            }

            public HttpBaseClientConfiguration HttpBaseClient { get; set; }

            public class HttpBaseClientConfiguration
            {
                public int TimeoutMilliseconds { get; set; } = 60000;

                public int RetryRequestTimeoutSeconds { get; set; } = 10;
                public int CacheClientConnectionSeconds { get; set; } = 300;
            }
        }

        public Configuration SystemLibraryCommonWeb { get; set; }
    }
}
