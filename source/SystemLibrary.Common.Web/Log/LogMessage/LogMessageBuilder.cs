using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web;

using static SystemLibrary.Common.Web.AppSettings.Configuration;

partial class Log
{
    static class LogMessageBuilder
    {
        static bool IsLocal;
        static LogMessageBuilderOptions LogMessageBuilderOptions;
        static LogMessageBuilder()
        {
            IsLocal = EnvironmentConfig.Current.IsLocal;
            LogMessageBuilderOptions = AppSettings.Current?.SystemLibraryCommonWeb?.LogMessageBuilder;
        }

        internal static string Get(object obj, LogLevel level)
        {
            var message = new StringBuilder("");

            if ((int)level != 99999)
                message.Append(level.ToString() + ": ");

            try
            {
                AppendMessage(obj, message, 0);

                if (!IsLocal && level == LogLevel.Error && obj as Exception == null)
                    AppendStackTrace(message);

                var context = HttpContextInstance.Current;
                if (context != null)
                    AppendRequestPath(message, context.Request);

                if (!IsLocal && context != null && LogMessageBuilderOptions != null)
                {
                    if (LogMessageBuilderOptions.AppendLoggedInState)
                        AppendLoggedInState(message, context);

                    if (level == LogLevel.Error)
                    {
                        if (LogMessageBuilderOptions.AppendBrowser)
                            AppendBrowser(message, context.Request);

                        if (LogMessageBuilderOptions.AppendIp)
                            AppendUserIp(message, context);

                        if (LogMessageBuilderOptions.AppendCookieInfo)
                            AppendCookieInfo(message, context.Request);
                    }
                }

                return message.ToString();
            }
            catch(Exception ex)
            {
                message.Append("Internal error: " + ex.Message + " when logging: " + obj);
                return message.ToString();
            }
        }

        static void AppendUserIp(StringBuilder message, HttpContext httpContext)
        {
            try
            {
                var connection = httpContext?.Connection;
                var remoteIpAddress = connection?.RemoteIpAddress;
                var localIpAddress = connection?.LocalIpAddress;

                if (remoteIpAddress == null && localIpAddress == null)
                {
                    message.Append("\nUser Ip: ");
                    return;
                }
                //httpContext?.Request?.Headers["HTTP_X_FORWARDED_FOR"];
                //httpContext?.Request?.Headers["REMOTE_ADDR"];

                var part1 = localIpAddress?.ToString();
                var part2 = remoteIpAddress?.ToString();

                if (part1.IsNot())
                    message.Append("\nUser Ip: " + part2);

                else if (part2.IsNot())
                    message.Append("\nUser Ip: " + part1);
                else
                    message.Append("\nUser Ip: " + part1 + ", " + part2);
            }
            catch
            {
                message.Append("\nUser Ip: unknown");
            }
        }

        static void AppendLoggedInState(StringBuilder message, HttpContext httpContext)
        {
            var isAuthenticated = httpContext?.User?.Identity?.IsAuthenticated == true;

            message.Append("\nIsLoggedIn: " + isAuthenticated);
        }

        static void AppendCookieInfo(StringBuilder message, HttpRequest request)
        {
            try
            {
                if (request?.Cookies?.Keys != null)
                {
                    message.Append("\nCookies: " + string.Join(", ", request.Cookies.Keys));
                }
            }
            catch
            {
            }
        }

        static void AppendMessage(object obj, StringBuilder message, int level)
        {
            if (level > 2) return;

            if (obj == null)
                message.Append("(null)");

            else if (obj is Exception ex)
            {
                message.Append(ex.Message);
                message.Append("\n" + ex.StackTrace);
            }

            else if (obj is ITuple ituple)
                message.Append(ituple[0] + ", " + (ituple?.Length > 1 ? ituple[1] + "" : ""));

            else if (obj is string txt)
                message.Append(txt);

            else if (obj is IEnumerable enumerable)
            {
                if (obj is IList list)
                    message.Append("List (" + list.Count + "): ");
                if (obj is Array array)
                    message.Append("Array (" + array.Length + "): ");
                if (obj is ICollection collection)
                    message.Append("Collection (" + collection.Count + "): ");

                if (obj is IDictionary dictionary)
                {
                    message.Append("Dictionary (" + dictionary.Count + "): ");
                    foreach (var value in dictionary.Values)
                        message.Append(value + " ");
                }
                else
                {
                    foreach (var val in enumerable)
                        AppendMessage(val, message, level++);
                }
            }
            else if (IsLogableClass(obj))
                AppendClass(message, obj);
            else
                message.Append(obj.ToString());
        }
    }

    static void AppendClass(StringBuilder message, object obj)
    {
        var dump = Type.GetType(typeName: "Dump, SystemLibrary.Common.Net");

        if (dump == null)
            throw new Exception("SystemLibrary.Common.Net.Dump is not loaded or type is renamed in version you are using");

        var method = dump.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
           .Where(x => x.Name == "Build")
           .FirstOrDefault();

        if (method == null)
            throw new Exception("Method 'Build' is renamed or do not exist");

        method.Invoke(null, new object[] { message, obj, 0, 3 });
    }

    static bool IsLogableClass(object o)
    {
        var type = o.GetType();

        return type.IsClass &&
            !type.IsEnum &&
            !type.IsArray &&
            type != SystemType.StringType &&
            type != SystemType.ExceptionType &&
            type != typeof(StringBuilder) &&
            type != typeof(Nullable) &&
            type != typeof(Nullable<>) &&
            type != typeof(NullReferenceException) &&
            type != typeof(RuntimeWrappedException);
    }


    static void AppendBrowser(StringBuilder message, HttpRequest request)
    {
        string userAgent = null;

        if (request?.Headers?.ContainsKey(HeaderNames.UserAgent) == true)
            userAgent = request.Headers[HeaderNames.UserAgent];

        message.Append("\nAgent: " + userAgent ?? "<empty>");
    }

    static void AppendStackTrace(StringBuilder message)
    {
        try
        {
            message.Append("\nStacktrace:");

            var stackTrace = Environment.StackTrace?.ToString();
            if (stackTrace != null)
            {
                var traces = stackTrace.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < Math.Min(traces.Length, 9); i++)
                {
                    if (traces[i].StartsWithAny(
                        "   at System.RuntimeMethodHandle",
                        "   at System.Reflection.RuntimeMethodInfo"))
                    {
                        break;
                    }

                    if (traces[i].StartsWithAny(
                        "   at Log.Write(Object obj, LogLevel level)",
                        "   at Log.LogMessageBuilder",
                        "   at System.Environment.get_",
                            "   at Log.AppendStackTrace"))
                    {
                        continue;
                    }


                    message.Append(traces[i] + "\n");
                }
            }
        }
        catch
        {
            message.Append(" unknown");
        }
    }

    static void AppendRequestPath(StringBuilder message, HttpRequest request)
    {
        if (request?.Path != null)
            message.Append("\nPath: " + request.Path.Value ?? "<empty>");
    }
}
