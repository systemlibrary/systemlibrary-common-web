namespace SystemLibrary.Common.Web;

partial class Client
{
    internal const int DefaultTimeout = 40001;
    internal const int DefaultRetryTimeout = 10000;
    internal const bool DefaultThrowOnUnsuccessful = true;
    internal const bool DefaultIgnoreSslErrors = true;
    internal const bool DefaultUseRetryPolicy = true;
    internal const bool DefaultUseRequestBreakerPolicy = false;
    internal const int DefaultClientCacheDuration = 1200; // 20 minutes

    static int? _TimeoutConfig;
    static int TimeoutConfig
    {
        get
        {
            if (_TimeoutConfig == null)
            {
                _TimeoutConfig = AppSettings.Current.SystemLibraryCommonWeb.Client.Timeout;

                if (_TimeoutConfig == null || _TimeoutConfig <= 0)
                {
                    _TimeoutConfig = DefaultTimeout;
                }
            }

            return _TimeoutConfig.Value;
        }
    }

    static int? _RetryTimeoutConfig;
    static int RetryTimeoutConfig
    {
        get
        {
            if (_RetryTimeoutConfig == null)
            {
                _RetryTimeoutConfig = AppSettings.Current.SystemLibraryCommonWeb.Client.RetryTimeout;

                if (_RetryTimeoutConfig == null || _RetryTimeoutConfig <= 0)
                {
                    _RetryTimeoutConfig = DefaultRetryTimeout;
                }
            }

            return _RetryTimeoutConfig.Value;
        }
    }

    static bool? _IgnoreSslErrorsConfig;
    static bool IgnoreSslErrorsConfig
    {
        get
        {
            if (_IgnoreSslErrorsConfig == null)
            {
                _IgnoreSslErrorsConfig = AppSettings.Current.SystemLibraryCommonWeb.Client.IgnoreSslErrors;

                if (_IgnoreSslErrorsConfig == null)
                {
                    _IgnoreSslErrorsConfig = DefaultIgnoreSslErrors;
                }
            }

            return _IgnoreSslErrorsConfig.Value;
        }
    }

    static bool? _UseRetryPolicyConfig;
    static bool UseRetryPolicyConfig
    {
        get
        {
            if (_UseRetryPolicyConfig == null)
            {
                _UseRetryPolicyConfig = AppSettings.Current.SystemLibraryCommonWeb.Client.UseRetryPolicy;

                if (_UseRetryPolicyConfig == null)
                {
                    _UseRetryPolicyConfig = DefaultUseRetryPolicy;
                }
            }

            return _UseRetryPolicyConfig.Value;
        }
    }

    static bool? _ThrowOnUnsuccessfulConfig;
    static bool ThrowOnUnsuccessfulConfig
    {
        get
        {
            if (_ThrowOnUnsuccessfulConfig == null)
            {
                _ThrowOnUnsuccessfulConfig = AppSettings.Current.SystemLibraryCommonWeb.Client.ThrowOnUnsuccessful;

                if (_ThrowOnUnsuccessfulConfig == null)
                {
                    _ThrowOnUnsuccessfulConfig = DefaultThrowOnUnsuccessful;
                }
            }

            return _ThrowOnUnsuccessfulConfig.Value;
        }
    }

    /// <summary>
    /// Request breaker triggers on 500, 502 and 504 errors
    /// </summary>
    static bool? _UseRequestBreakerPolicyConfig;
    static bool UseRequestBreakerPolicyConfig
    {
        get
        {
            if (_UseRequestBreakerPolicyConfig == null)
            {
                _UseRequestBreakerPolicyConfig = AppSettings.Current.SystemLibraryCommonWeb.Client.UseRequestBreakerPolicy;

                if (_UseRequestBreakerPolicyConfig == null)
                {
                    _UseRequestBreakerPolicyConfig = DefaultUseRequestBreakerPolicy;
                }
            }

            return _UseRequestBreakerPolicyConfig.Value;
        }
    }

    /// <summary>
    /// The amount of time the HttpClient is cached inside of CacheModel
    /// - HttpClient natively supports "infinite", but we recreate it every now and then
    /// - We expire the whole CacheModel which wraps our HttpClient after this duration and mark it for disposal
    /// </summary>
    static int? _ClientCacheDurationConfig;
    static int ClientCacheDurationConfig
    {
        get
        {
            if (_ClientCacheDurationConfig == null)
            {
                _ClientCacheDurationConfig = AppSettings.Current.SystemLibraryCommonWeb.Client.ClientCacheDuration;

                if (_ClientCacheDurationConfig == null || _ClientCacheDurationConfig < 0)
                {
                    _ClientCacheDurationConfig = DefaultClientCacheDuration;
                }
            }

            return _ClientCacheDurationConfig.Value;
        }
    }
}
