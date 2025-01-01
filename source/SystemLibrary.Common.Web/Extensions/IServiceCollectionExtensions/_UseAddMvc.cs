using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddMvc(this IServiceCollection services, ServicesCollectionOptions options)
    {
        var builder = services.AddMvc();

        builder.Services.Configure(ConfigureSupportedMediaTypes(options));

        return builder;
    }
}