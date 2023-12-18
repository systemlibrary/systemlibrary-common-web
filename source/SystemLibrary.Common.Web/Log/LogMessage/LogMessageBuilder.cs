using System;
using System.Collections;
using System.Collections.Generic;
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
        static bool MessageFormatJson;
        static LogMessageBuilder()
        {
            IsLocal = EnvironmentConfig.IsLocal == true;
            LogMessageBuilderOptions = AppSettings.Current?.SystemLibraryCommonWeb?.LogMessageBuilder;
            MessageFormatJson = LogMessageBuilderOptions.Format?.Trim()?.ToLower() == "json";
        }

        internal static string Get(object obj, LogLevel level)
        {
            var message = new StringBuilder();
            if (MessageFormatJson)
                message.Append("{\n");

            try
            {
                if ((int)level != 99999)
                    AppendMessageFormat("level", level, message);

                if (obj is string json && json.IsJson())
                    AppendMessageFormat("message", json, message);
                else
                    AppendMessage(obj, message, MessageFormatJson ? 1 : 0);

                if (!IsLocal && level == LogLevel.Error && obj as Exception == null)
                    AppendStackTrace(message);

                var context = HttpContextInstance.Current;

                if (!IsLocal && context != null)
                {
                    if(LogMessageBuilderOptions.AppendPath)
                        AppendRequestPath(message, context.Request);

                    if (LogMessageBuilderOptions != null)
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

                    if (LogMessageBuilderOptions.AppendCorrelationId)
                        AppendCorrelationId(message, context);
                }
            }
            catch (Exception ex)
            {
                AppendMessageFormat("logMessageError", ex.Message + " when logging: " + obj, message);
            }

            if (MessageFormatJson)
                message.Append("}\n");

            return message.ToString();
        }

        static void AppendCorrelationId(StringBuilder message, HttpContext httpContext)
        {
            try
            {
                if (httpContext?.Items == null) return;

                var name = "CorrelationId";
                var id = httpContext.Items[name];

                if (id == null)
                {
                    name = "correlationId";
                    id = httpContext.Items[name];

                    if (id == null)
                    {
                        name = "correlationid";
                        id = httpContext.Items[name];
                    }

                    if (id == null)
                    {
                        name = "CorrelationID";
                        id = httpContext.Items[name];
                    }

                    if (id == null)
                    {
                        name = "corrId";
                        id = httpContext.Items[name];
                    }
                }

                if (id != null)
                    AppendMessageFormat(name, id, message);
                else
                {
                    var correlation = Guid.NewGuid().ToString();

                    httpContext.Items.TryAdd(name, correlation);

                    AppendMessageFormat(name, correlation, message);
                }
            }
            catch
            {
                AppendMessageFormat("correlationId", null, message);
            }
        }

        static void AppendUserIp(StringBuilder message, HttpContext httpContext)
        {
            try
            {
                var connection = httpContext?.Connection;

                if (connection == null) return;

                var remoteIpAddress = connection?.RemoteIpAddress;

                var userIp = remoteIpAddress?.ToString();

                var wasLocal = false;

                if (userIp.IsNot() || userIp == "::1" || userIp.StartsWith("10.") || userIp.StartsWith("127.0"))
                {
                    userIp = httpContext.Request?.Headers["X-Forwarded-For"].FirstOrDefault();
                    wasLocal = true;
                }

                if (userIp.IsNot() || userIp == "::1" || userIp.StartsWith("10.") || userIp.StartsWith("127.0"))
                {
                    userIp = httpContext.Request?.Headers["REMOTE_ADDR"].FirstOrDefault();
                    wasLocal = true;
                }

                if (userIp?.Contains(",") == true)
                {
                    userIp = userIp.Split(',')[0];
                }

                if (userIp.IsNot())
                {
                    if(wasLocal)
                        AppendMessageFormat("ip", "local", message);
                    else
                        AppendMessageFormat("ip", "empty", message);
                }

                else if (userIp == "::1" || userIp.StartsWith("10.") || userIp.StartsWith("127.0"))
                    AppendMessageFormat("ip", "local", message);

                else
                    AppendMessageFormat("ip", userIp, message);
            }
            catch
            {
                AppendMessageFormat("ip", "unknown", message);
            }
        }

        static void AppendLoggedInState(StringBuilder message, HttpContext httpContext)
        {
            var isAuthenticated = httpContext?.User?.Identity?.IsAuthenticated == true;

            AppendMessageFormat("authenticated", isAuthenticated, message);
        }

        static void AppendCookieInfo(StringBuilder message, HttpRequest request)
        {
            try
            {
                if (request?.Cookies?.Keys != null)
                {
                    AppendMessageFormat("cookies", string.Join(", ", request.Cookies.Keys), message);
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
                AppendMessageFormat("object", null, message, level);

            else if (obj is Exception ex)
            {
                AppendMessageFormat("exception", ex.Message + "\n" + ex.StackTrace, message, level);
            }

            else if (obj is ITuple ituple)
                AppendMessageFormat("tuple", ituple[0] + ", " + (ituple?.Length > 1 ? ituple[1] + "" : ""), message, level);

            else if (obj is string txt)
                AppendMessageFormat("message", txt, message, level);

            else if (obj is IEnumerable enumerable)
                AppendClass(message, enumerable);
            else if (IsLoggableClass(obj))
                AppendClass(message, obj);
            else
                AppendMessageFormat("message", obj.ToString(), message);
        }

        static void AppendMessageFormat(string name, object obj, StringBuilder message, int level = -1)
        {
            if (level > 2) return;

            if (MessageFormatJson)
                AppendJsonFormat(name, obj, message);
            else
                AppendPlainFormat(name, obj, message);
        }

        static void AppendJsonFormat(string name, object obj, StringBuilder message)
        {
            if (obj == null)
                message.Append("\t\"" + name + "\": null,\n");
            else
                message.Append("\t\"" + name + "\": \"" + obj + "\",\n");
        }

        static void AppendPlainFormat(string name, object obj, StringBuilder message)
        {
            if (name == "level")
            {
                message.Append(obj + ": ");
            }
            else if (name == "message")
            {
                if (obj == null)
                    message.Append("(null)\n");
                else
                    message.Append(obj + "\n");
            }
            else if (obj == null)
                message.Append(name + ": null\n");
            else if (name == "stacktrace")
                message.Append(name + ": " + obj + "\n");
            else
                message.Append(name + ": " + obj + "\n");
        }

        static MethodInfo DumpBuildMethod;
        static void AppendClass(StringBuilder message, object obj)
        {
            if (DumpBuildMethod == null)
            {
                var type = Type.GetType(typeName: "Dump, SystemLibrary.Common.Net");

                if (type == null)
                    throw new Exception("SystemLibrary.Common.Net.Dump is not loaded or type is renamed in version you are using");

                DumpBuildMethod = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                   .Where(x => x.Name == "Build")
                   .FirstOrDefault();

                if (DumpBuildMethod == null)
                    throw new Exception("Method 'Build' is renamed or do not exist");
            }

            var objAsString = new StringBuilder();

            try
            {
                DumpBuildMethod.Invoke(null, new object[] { objAsString, obj, 0, 3 });
            }
            catch
            {
            }

            AppendMessageFormat("object", objAsString.ToString(), message);
        }

        static bool IsLoggableClass(object o)
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

            AppendMessageFormat("agent", userAgent, message);
        }

        static void AppendStackTrace(StringBuilder message)
        {
            try
            {
                var stackTraceBuilder = new StringBuilder("");
                var stackTrace = Environment.StackTrace?.ToString();
                if (stackTrace != null)
                {
                    var traces = stackTrace.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < Math.Min(traces.Length, 9); i++)
                    {
                        if (traces[i].StartsWithAny(
                            "   at System.RuntimeMethodHandle",
                            "   at System.Reflection.RuntimeMethodInfo",
                            "   at lambda_method"))
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

                        if (i < Math.Min(traces.Length, 9) - 1)
                        {
                            if(MessageFormatJson)
                                stackTraceBuilder.Append("\t" + traces[i].TrimStart() + "\n");
                            else
                            {
                                stackTraceBuilder.Append("\t" + traces[i].TrimStart() + "\n");
                            }
                        }
                            
                    }
                }
                AppendMessageFormat("stacktrace", stackTraceBuilder.ToString(), message);
            }
            catch
            {
                AppendMessageFormat("stacktrace", "unknown", message);
            }
        }

        static void AppendRequestPath(StringBuilder message, HttpRequest request)
        {
            if (request?.Path != null)
                AppendMessageFormat("path", request.Path.Value + (request.QueryString.Value), message);
        }
    }
}
