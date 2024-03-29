using System;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder AddApplicationPart(IMvcBuilder builder, CommonWebServicesOptions options, Assembly executing, Assembly entry)
    {
        if (builder != null)
        {
            if (executing != null)
                builder = builder.AddApplicationPart(executing);

            if (entry != null && executing?.FullName != entry.FullName)
                builder = builder.AddApplicationPart(entry);
        }
        else if (options.SupportedMediaTypes != null)
            throw new Exception("ConfigureMvc, ConfigureRazorPages and ConfigureControllers are false, yet you've set SupportedMediaTypes or registered EndpointAssemblies. Set one of the flags to true or registered the requires services yourself");

        return builder;
    }
}