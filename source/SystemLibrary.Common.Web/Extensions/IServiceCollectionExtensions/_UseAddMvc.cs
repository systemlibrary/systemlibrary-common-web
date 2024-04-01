using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddMvc(this IServiceCollection services, ServicesCollectionOptions options)
    {
        return services.AddMvc(ConfigureSupportedMediaTypes(options));
    }
}