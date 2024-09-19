namespace SystemLibrary.Common.Web;

partial class Cache
{
    internal const int DefaultDuration = 180;
    internal const int DefaultFallbackDuration = 600;
    internal const int DefaultContainerSizeLimit = 40000;

    static int? _DurationConfig;
    static int DurationConfig
    {
        get
        {
            if (_DurationConfig == null)
            {
                _DurationConfig = AppSettings.Current.SystemLibraryCommonWeb.Cache.Duration;

                if (_DurationConfig == null || _DurationConfig <= 0)
                {
                    _DurationConfig = DefaultDuration;
                }
            }

            return _DurationConfig.Value;
        }
    }

    static int? _FallbackDurationConfig;
    static int FallbackDurationConfig
    {
        get
        {
            if (_FallbackDurationConfig == null)
            {
                _FallbackDurationConfig = AppSettings.Current.SystemLibraryCommonWeb.Cache.FallbackDuration;

                if (_FallbackDurationConfig == null || _FallbackDurationConfig < 0)
                {
                    _FallbackDurationConfig = DefaultFallbackDuration;
                }
            }

            return _FallbackDurationConfig.Value;
        }
    }

    static int? _ContainerSizeLimitConfig;
    static int ContainerSizeLimitConfig
    {
        get
        {
            if (_ContainerSizeLimitConfig == null)
            {
                _ContainerSizeLimitConfig = AppSettings.Current.SystemLibraryCommonWeb.Cache.ContainerSizeLimit;

                if (_ContainerSizeLimitConfig == null || _ContainerSizeLimitConfig < 10)
                {
                    _ContainerSizeLimitConfig = DefaultContainerSizeLimit;
                }
            }

            return _ContainerSizeLimitConfig.Value;
        }
    }
}
