namespace SystemLibrary.Common.Web;

/// <summary>
/// ILogWriter is responsible to store the log messages inside your application
/// 
/// - Create a new class and implement the interface
/// - Register the interface and class as a 'service' in your 'Startup.cs'
/// - Calling any of the public Log.() methods will send the log message to your log writer which you control where to store
/// </summary>
/// <example>
/// <code class="language-csharp hljs">
/// public class LogWriter : ILogWriter
/// { 
///     public void Error(string message) 
///     {
///     }
///     ...
/// }
/// 
/// //Inside your startup/initialize class
/// public void ConfigureServices(IServiceCollection services)
/// {
///    var options = new CommonWebApplicationServicesOptions();
///    services.CommonWebApplicationServices(options);
///    services.AddTransient&lt;ILogWriter, LogWriter&gt;();
///    //all other services you want to add...
/// }
/// 
/// //Usage
/// Log.Error("hello world");
/// Log.Warning("hello world");
/// Log.Debug("hello world");
/// Log.Info("hello world");
/// Log.Write("hello world");
/// </code>
/// </example>
public interface ILogWriter
{
    /// <summary>
    /// Implement where Log.Error() is going to be stored, for instance your own DB, a file, or some services like CloudWatch, FireBase, Sentry, ...
    /// </summary>
    void Error(string message);

    /// <summary>
    /// Implement where Log.Warning() is going to be stored, for instance your own DB, a file, or some services like CloudWatch, FireBase, Sentry, ...
    /// </summary>
    void Warning(string message);

    /// <summary>
    /// Implement where Log.Debug() is going to be stored, for instance your own DB, a file, or some services like CloudWatch, FireBase, Sentry, ...
    /// </summary>
    void Debug(string message);

    /// <summary>
    /// Implement where Log.Info() is going to be stored, for instance your own DB, a file, or some services like CloudWatch, FireBase, Sentry, ...
    /// </summary>
    void Info(string message);

    /// <summary>
    /// Implement where Log.Write() is going to be stored, for instance your own DB, a file, or some services like CloudWatch, FireBase, Sentry, ...
    /// 
    /// Write() is always Invoked, no matter if you disable logging or set to invald log levels, it will be logged
    /// </summary>
    void Write(string message);
}
