using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddMvc(this IServiceCollection services, CommonWebApplicationServicesOptions options)
    {
        return services.AddMvc(UseDefaultSupportedMediaTypes(options));
    }
}