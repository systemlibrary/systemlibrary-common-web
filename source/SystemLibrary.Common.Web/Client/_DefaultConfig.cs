namespace SystemLibrary.Common.Web;

partial class Client
{
    internal const int DefaultTimeout = 40000;
    internal const int DefaultRetryTimeout = 10000;
    internal const bool DefaultThrowOnUnsuccessful = false;
    internal const bool DefaultIgnoreSslErrors = false;
    internal const bool DefaultUseRetryPolicy = true;
    internal const bool DefaultUseCircuitBreakerPolicy = false;
    internal const int ClientCacheDuration = 720000;

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

    static bool? _UseCircuitBreakerPolicyConfig;
    static bool UseCircuitBreakerPolicyConfig
    {
        get
        {
            if (_UseCircuitBreakerPolicyConfig == null)
            {
                _UseCircuitBreakerPolicyConfig = AppSettings.Current.SystemLibraryCommonWeb.Client.UseCircuitBreakerPolicy;

                if (_UseCircuitBreakerPolicyConfig == null)
                {
                    _UseCircuitBreakerPolicyConfig = DefaultUseCircuitBreakerPolicy;
                }
            }

            return _UseCircuitBreakerPolicyConfig.Value;
        }
    }


}
