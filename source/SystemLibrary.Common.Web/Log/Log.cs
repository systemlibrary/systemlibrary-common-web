using SystemLibrary.Common.Web;

/// <summary>
/// One log class for your whole application
/// 
/// This lives in the Global Namespace - so it is available inside your views, controllers, services, anywhere, as long as the nuget package is added
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
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// Log.Error("hello world");
    /// //This creates a log message with prefix 'Error', timestamp, stacktrace and your input text "hello world" and sends it to your LogWriter
    /// </code>
    /// </example>
    public static void Error(object obj)
    {
        Write(obj, LogLevel.Error);
    }

    /// <summary>
    /// Write a warning message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// Log.Warning("hello world");
    /// //This creates a log message with prefix 'Warning', timestamp and your input text "hello world" and sends it to your LogWriter
    /// </code>
    /// </example>
    public static void Warning(object obj)
    {
        Write(obj, LogLevel.Warning);
    }

    /// <summary>
    /// Write a debug message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// Log.Debug("hello world");
    /// //This creates a log message with prefix 'Debug', timestamp and your input text "hello world" and sends it to your LogWriter
    /// </code>
    /// </example>
    public static void Debug(object obj)
    {
        Write(obj, LogLevel.Debug);
    }


    /// <summary>
    /// Write an information message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// Log.Info("hello world");
    /// //This creates a log message with prefix 'Info', timestamp and your input text "hello world" and sends it to your LogWriter
    /// </code>
    /// </example>
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
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// var list = new List&lt;User&gt;
    /// list.Add(new User { firstName = "hello", LastName = "World" });
    /// 
    /// Log.Write(list);
    /// //This creates a log message with your input and sends it as a string to your LogWriter
    /// //Note Log.Write() can never be disabled/turned off, remove the calls if you dont want them in production
    /// </code>
    /// </example>
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


namespace SystemLibrary.Common.Web.Global
{
    /// <summary>
    /// One log class for your whole application
    /// 
    /// This lives in the Global Namespace - so it is available inside your views, controllers, services, anywhere, as long as the nuget package is added
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
        /// <summary>
        /// Write an error message
        /// </summary>
        /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
        /// <example>
        /// Usage:
        /// <code class="language-csharp hljs">
        /// Log.Error("hello world");
        /// //This creates a log message with prefix 'Error', timestamp, stacktrace and your input text "hello world" and sends it to your LogWriter
        /// </code>
        /// </example>
        public static void Error(object obj)
        {
            throw new System.Exception("Dot not use the SystemLibrary.Common.Web.Global namespace directly, it's fake, just for the documentation. Use Log from the global C# namespace, google it");
        }

        /// <summary>
        /// Write a warning message
        /// </summary>
        /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
        /// <example>
        /// Usage:
        /// <code class="language-csharp hljs">
        /// Log.Warning("hello world");
        /// //This creates a log message with prefix 'Warning', timestamp and your input text "hello world" and sends it to your LogWriter
        /// </code>
        /// </example>
        public static void Warning(object obj)
        {
            throw new System.Exception("Dot not use the SystemLibrary.Common.Web.Global namespace directly, it's fake, just for the documentation. Use Log from the global C# namespace, google it");
        }

        /// <summary>
        /// Write a debug message
        /// </summary>
        /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
        /// <example>
        /// Usage:
        /// <code class="language-csharp hljs">
        /// Log.Debug("hello world");
        /// //This creates a log message with prefix 'Debug', timestamp and your input text "hello world" and sends it to your LogWriter
        /// </code>
        /// </example>
        public static void Debug(object obj)
        {
            throw new System.Exception("Dot not use the SystemLibrary.Common.Web.Global namespace directly, it's fake, just for the documentation. Use Log from the global C# namespace, google it");
        }


        /// <summary>
        /// Write an information message
        /// </summary>
        /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
        /// <example>
        /// Usage:
        /// <code class="language-csharp hljs">
        /// Log.Info("hello world");
        /// //This creates a log message with prefix 'Info', timestamp and your input text "hello world" and sends it to your LogWriter
        /// </code>
        /// </example>
        public static void Info(object obj)
        {
            throw new System.Exception("Dot not use the SystemLibrary.Common.Web.Global namespace directly, it's fake, just for the documentation. Use Log from the global C# namespace, google it");
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
        /// <example>
        /// Usage:
        /// <code class="language-csharp hljs">
        /// var list = new List&lt;User&gt;
        /// list.Add(new User { firstName = "hello", LastName = "World" });
        /// 
        /// Log.Write(list);
        /// //This creates a log message with your input and sends it as a string to your LogWriter
        /// //Note Log.Write() can never be disabled/turned off, remove the calls if you dont want them in production
        /// </code>
        /// </example>
        public static void Write(object obj)
        {
            throw new System.Exception("Dot not use the SystemLibrary.Common.Web.Global namespace directly, it's fake, just for the documentation. Use Log from the global C# namespace, google it");
        }
    }

    /// <summary>
    /// The log levels you can chose between
    /// </summary>
    public enum LogLevel
    {
        Info = 1,
        Debug,
        Warning,
        Error
    }
}