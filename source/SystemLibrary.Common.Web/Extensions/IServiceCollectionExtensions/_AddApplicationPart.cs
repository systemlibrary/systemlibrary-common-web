using System;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

static partial class IServiceCollectionExtensions
{
    static IMvcBuilder AddApplicationPart(IMvcBuilder builder, CommonWebApplicationServicesOptions options)
    {
        if (builder != null)
        {
            var executingAssembliy = Assembly.GetExecutingAssembly();
            var entryAssembly = Assembly.GetEntryAssembly();

            if (executingAssembliy != null)
                builder = builder.AddApplicationPart(executingAssembliy);

            if (executingAssembliy?.FullName != entryAssembly?.FullName)
                builder = builder.AddApplicationPart(entryAssembly);
        }
        else if (options.SupportedMediaTypes != null)
            throw new Exception("AddMvcPages, AddRazorPages and AddControllers are false, yet you've set SupportedMediaTypes. Either set one of the flags to true, or register SupportedMediaTypes yourself");

        return builder;
    }
}