﻿using System.Security.Principal;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Extension methods on System.Security.Principal.IPrincipal
/// </summary>
public static class IPrincipalExtensions
{
    /// <summary>
    /// Check if principal is in any role, case sensitive
    /// 
    /// Returns true if so, else false
    /// </summary>
    /// <example>
    /// Usage:
    /// <code>
    /// var roles = new object[] { "Admin", "Guest" };
    /// principal.IsInAnyRole(roles);
    /// </code>
    /// </example>
    public static bool IsInAnyRole(this IPrincipal principal, params object[] roles)
    {
        if (principal == null || roles == null) return false;

        for (int i = 0; i < roles.Length; i++)
            if (roles[i] != null && principal.IsInRole(roles[i] + ""))
                return true;

        return false;
    }
}
