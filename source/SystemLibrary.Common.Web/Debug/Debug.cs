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

    internal static void Log(string msg)
    {
        if (Debugging)
        {
            global::Log.Debug("Debug Web 'true': " + msg);
        }
    }
}
