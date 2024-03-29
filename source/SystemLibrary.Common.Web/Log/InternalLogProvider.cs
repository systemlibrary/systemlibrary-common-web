using Microsoft.Extensions.Logging;

namespace SystemLibrary.Common.Web;

internal class InternalLogProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new InternalLog();
    }

    public void Dispose()
    {
        // Dispose anyresources used by the logger provider
    }
}
