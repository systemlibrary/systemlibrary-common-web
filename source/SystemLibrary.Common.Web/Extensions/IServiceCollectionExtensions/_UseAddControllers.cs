using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddControllers(this IServiceCollection services, CommonWebServicesOptions options)
    {
        return services.AddControllersWithViews(UseDefaultSupportedMediaTypes(options));
    }
}