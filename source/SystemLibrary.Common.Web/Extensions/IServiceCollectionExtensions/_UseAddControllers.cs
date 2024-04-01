using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddControllers(this IServiceCollection services, ServicesCollectionOptions options)
    {
        return services.AddControllersWithViews(ConfigureSupportedMediaTypes(options));
    }
}