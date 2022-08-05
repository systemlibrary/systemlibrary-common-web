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
///     var options = new CommonWebApplicationBuilderOptions();
///     options.UseHttpRedrectionAndHsts = false;
///     app.CommonAppBuilder(options);
/// }
/// </code>
/// </example>
public class CommonWebApplicationBuilderOptions
{
    /// <summary>
    /// Adds middleware for http to https redirect
    /// </summary>
    public bool UseHttpRedirectionAndHsts = true;

    /// <summary>
    /// Adds the default routing middleware that comes with Aspnet
    /// </summary>
    public bool UseDefaultRouting = true;

    /// <summary>
    /// Adds middleware for Authorization and Authentication attributes
    /// </summary>
    public bool UseAuthenticationAndAuthorization = true;

    /// <summary>
    /// Adds middleware to route urls to controllers
    /// </summary>
    public bool UseControllerEndpoints = true;

    /// <summary>
    /// Adds middleware for razor pages as endpoints aka 'MapRazorPages'
    /// </summary>
    public bool UseRazorPagesEndpoints = true;

    /// <summary>
    /// Adds middleware for static files and sets a few default settings:
    /// - allows serving of unknown files types
    /// - compression is set to 'HttpsCompressionMode.Compress' 
    /// - does not append a trailing slash for static files
    /// </summary>
    public bool UseStaticFiles = true;

    /// <summary>
    /// Adds middleware which responds with a exception page
    /// - The exception page is never shown if Environment.IsProd is true, unless you set this to false and you call on 'UseDeveloperExceptionPage' yourself
    /// - Unless you set this flag to false, and register the Dev page yourself under your conditions
    /// </summary>
    public bool UseExceptionPageInTestAndDev = true;

    /// <summary>
    /// Adds middleware for cookie policies
    /// </summary>
    public bool UseHttpsAndSecureCookiePolicy { get; set; } = true;
}
