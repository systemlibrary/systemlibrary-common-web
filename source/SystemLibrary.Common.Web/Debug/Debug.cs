namespace SystemLibrary.Common.Web;

internal static class Debug
{
    static bool? _Debugging;

    static bool Debugging
    {
        get
        {
            _Debugging ??= AppSettings.Current?.SystemLibraryCommonWeb?.Debug == true;

            return _Debugging.Value;
        }
    }

    // Requires debug=true and log level=debug or lower to be able to print debug information
    internal static void Log(string message)
    {
        if (Debugging)
        {
            global::Log.Debug(message);
        }
    }
}
