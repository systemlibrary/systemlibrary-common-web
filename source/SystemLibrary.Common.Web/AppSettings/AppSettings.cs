using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web;

internal class AppSettings : Config<AppSettings>
{
    public AppSettings()
    {
        SystemLibraryCommonWeb = new PackageConfig();
        Logging = new Logging();
    }
    public PackageConfig SystemLibraryCommonWeb { get; set; }
    public Logging Logging { get; set; }
}

internal class LoggingLogLevel
{
    public string Default { get; set; } = "Information";
}

internal class Logging
{
    public LoggingLogLevel LogLevel { get; set; }
    public Logging()
    {
        LogLevel = new LoggingLogLevel();
    }
}

