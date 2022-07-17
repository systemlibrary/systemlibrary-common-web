namespace SystemLibrary.Common.Web;

/// <summary>
/// One log class for your whole application
/// 
/// - You implement the actual LogWriter : ILogWriter of where the message should be stored
/// - Remember to register your LogWriter as a service in your application
/// - After implementing ILogWriter, simply call Log.Error, Log.Warning or Log.Write to create a log message and it will passed to your implementation of ILogWriter
/// 
/// - If you have not registered an ILogWriter interface this simply calls on Dump.Write() with the message
/// </summary>
/// <example>
/// Configure log options in appSettings.json
/// <code class="language-csharp hljs">
/// {
///     "systemLibraryCommonWeb": {
///         "log" { 
///             "level": "0/info/Info/debug/Debug/warning/Warning/Error/error",
///             "isEnabled": true/false
///         },
///         "logMessageBuilderOptions" {
///             "appendLoggedInState": true,
///             "appendLoggedInState": true,
///             "appendCurrentPage": true,
///             "appendCurrentUrl": true,
///             "appendIp": true,
///             "appendBrowser": true,
///             "appendCookieInfo": true
///         }
///     }
/// }
/// </code>
/// </example>
public static partial class Log
{
    static ILogWriter _LogWriter;
    static ILogWriter LogWriter
    {
        get
        {
            if(_LogWriter == null)
                _LogWriter = Services.Get<ILogWriter>();

            return _LogWriter;
        }
    }

    /// <summary>
    /// Write an error message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    public static void Error(object obj)
    {
        Write(obj, LogLevel.Error);
    }

    /// <summary>
    /// Write a warning message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    public static void Warning(object obj)
    {
        Write(obj, LogLevel.Warning);
    }

    /// <summary>
    /// Write a debug message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    public static void Debug(object obj)
    {
        Write(obj, LogLevel.Debug);
    }


    /// <summary>
    /// Write an information message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    public static void Info(object obj)
    {
        Write(obj, LogLevel.Info);
    }

    /// <summary>
    /// Writing the object to your logwriter
    /// 
    /// - This ignores logging 'isEnabled' option in appSettings
    /// - This ignores 'log level' option in appSettings
    /// 
    /// This simply logs no matter what
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    public static void Write(object obj)
    {
        Write(obj, (LogLevel)99999);
    }

    static void Write(object obj, LogLevel level)
    {
        if ((int)level != 99999)
        {
            var isEnabled = AppSettings.Current.SystemLibraryCommonWeb.Log.IsEnabled;

            if (!isEnabled) return;

            var minLevel = AppSettings.Current.SystemLibraryCommonWeb.Log.Level;

            if((int)level < (int)minLevel)
                return;
        }

        var message = LogMessageBuilder.Get(obj, level);

        if (LogWriter == null)
        {
            Dump.Write("Warn: SystemLibrary.Common.Web.ILogWriter is not implemented and registered as a service.");
            Dump.Write(message);
            return;
        }

        switch (level)
        {
            case LogLevel.Error:
                LogWriter.Error(message);
                break;
            case LogLevel.Warning:
                LogWriter.Warning(message);
                break;
            case LogLevel.Debug:
                LogWriter.Debug(message);
                break;
            case LogLevel.Info:
                LogWriter.Info(message);
                break;
            default:
                LogWriter.Write(message);
                break;
        }
    }
}

public enum LogLevel
{
    Info = 1,
    Debug,
    Warning,
    Error
}
