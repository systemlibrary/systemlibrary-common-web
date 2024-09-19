using System;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Web.Extensions;

static partial class IServiceCollectionExtensions
{
    static Action<MvcOptions> ConfigureSupportedMediaTypes(ServicesCollectionOptions options)
    {
        return mvc =>
        {
            mvc.OutputFormatters.Add(new DefaultSupportedMediaTypes());

            if (options.AdditionalSupportedMediaTypes != null)
                mvc.OutputFormatters.Add(options.AdditionalSupportedMediaTypes);
        };
    }
}