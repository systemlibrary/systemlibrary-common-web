﻿using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    static void AddRazorRuntimeCompilationOnChange(IMvcBuilder builder)
    {
        if (builder != null)
            builder.AddRazorRuntimeCompilation();
        else
            throw new System.Exception("RazorRuntimeCompilation was not registered, as you've set Controllers, Mvc and RazorPages to false. You must manually register it yourself");
    }
}