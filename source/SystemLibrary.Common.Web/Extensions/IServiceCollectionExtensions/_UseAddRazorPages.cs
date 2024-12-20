using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

static partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddRazorPages(this IServiceCollection services, ServicesCollectionOptions options = null, IMvcBuilder builder = null)
    {
        if (builder == null)
            builder = services.AddRazorPages();

        builder.Services.Configure(ConfigureSupportedMediaTypes(options));

        return builder;
    }
}