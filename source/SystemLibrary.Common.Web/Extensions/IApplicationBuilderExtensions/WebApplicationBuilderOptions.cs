namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Application builder options
/// 
/// For instance: control wether or not you want to force http redirection to https (both server and client side)
/// </summary>
/// <example>
/// Inside your 'Startup' class:
/// <code>
/// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
/// {
///     var options = new ApplicationBuilderOptions();
///     options.UseHttpRedrectionAndHsts = false;
///     app.CommonAppBuilder(options);
/// }
/// </code>
/// </example>
public class WebApplicationBuilderOptions
{
    public bool UseHttpRedirectionAndHsts = true;
    public bool UseDefaultRouting = true;
    public bool UseAuthenticationAndAuthorization = true;
    public bool UseControllerEndpoints = true;
    public bool UseRazorPagesEndpoints = true;
    public bool UseStaticFiles = true;
    public bool UseExceptionPageInTestAndDev = true;
    public bool UseHttpsAndSecureCookiePolicy { get; set; } = true;
}
