﻿using System;
using System.Text;

namespace SystemLibrary.Common.Web;

internal static class PolicyKeyConverter
{
    internal static string Convert(Client.RequestOptions options)
    {
        var uri = new Uri(options.Url);

        var sb = new StringBuilder(128);

        sb.Append($"{options.Method}{uri.Scheme.ToLower()}{uri.Authority.ToLower()}{uri.Port}");

        AppendAbsolutePathAsKey(sb, uri.AbsolutePath);

        sb.Append(HttpContextInstance.Current?.User?.Identity?.IsAuthenticated + "" ?? "False");

        return sb.ToString();
    }

    static void AppendAbsolutePathAsKey(StringBuilder sb, string path)
    {
        if (path.IsNot()) return;

        if (path.Length < 6)
        {
            sb.Append(path.ToLower());
            return;
        }

        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
            sb.Append(part.MaxLength(12).ToLower());
    }
}