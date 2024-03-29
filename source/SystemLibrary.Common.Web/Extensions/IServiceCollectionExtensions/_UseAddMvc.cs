using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddMvc(this IServiceCollection services, CommonWebServicesOptions options)
    {
        return services.AddMvc(UseDefaultSupportedMediaTypes(options));
    }
}