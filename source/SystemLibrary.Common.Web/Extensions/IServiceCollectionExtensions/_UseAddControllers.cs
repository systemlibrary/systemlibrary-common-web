using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddControllers(this IServiceCollection services, CommonWebApplicationServicesOptions options)
    {
        return services.AddControllersWithViews(UseDefaultSupportedMediaTypes(options));
    }
}