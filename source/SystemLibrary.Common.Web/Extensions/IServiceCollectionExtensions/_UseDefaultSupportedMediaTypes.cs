using System;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Web.Extensions;

static partial class IServiceCollectionExtensions
{
    static Action<MvcOptions> UseDefaultSupportedMediaTypes(CommonWebApplicationServicesOptions options)
    {
        return mvc =>
        {
            mvc.OutputFormatters.Add(new DefaultSupportedMediaTypes());

            if (options.SupportedMediaTypes != null)
                mvc.OutputFormatters.Add(options.SupportedMediaTypes);
        };
    }
}