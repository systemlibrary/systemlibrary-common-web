using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

static partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddControllers(this IServiceCollection services, CommonWebApplicationServicesOptions options)
    {
        return services.AddControllersWithViews(UseDefaultSupportedMediaTypes(options));
    }
}