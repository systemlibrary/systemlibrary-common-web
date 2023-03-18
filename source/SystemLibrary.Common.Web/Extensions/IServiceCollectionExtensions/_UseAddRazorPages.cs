using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

static partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddRazorPages(this IServiceCollection services, CommonWebApplicationServicesOptions options = null)
    {
        var builder = services.AddRazorPages();

        builder.Services.Configure(UseDefaultSupportedMediaTypes(options));

        return builder;
    }
}