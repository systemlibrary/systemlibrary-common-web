using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseCookiePolicy(this IServiceCollection services)
    {
        return services.Configure<CookiePolicyOptions>(options =>
        {
            options.HttpOnly = HttpOnlyPolicy.Always;

            options.Secure = CookieSecurePolicy.SameAsRequest;

            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
    }
}