namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Web Application Builder Options
/// 
/// All options are 'true' (on) by default
/// 
/// - Used as argument in 'app.CommonWebApplicationBuilder' function
/// </summary>
/// <example>
/// Startup.cs/Program.cs:
/// <code>
/// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
/// {
///     var options = new CommonWebApplicationBuilderOptions();
///     
///     options.UseHttpRedrectionAndHsts = false;
///     
///     app.CommonWebApplicationBuilder(options);
/// }
/// </code>
/// </example>
public class CommonWebApplicationBuilderOptions
{
    /// <summary>
    /// Adds middleware for http to https redirect
    /// </summary>
    public bool UseHttpToHttpsRedirectionAndHsts = true;

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
    public bool MapControllerEndpoints = true;

    /// <summary>
    /// Adds middleware for razor pages as endpoints aka 'MapRazorPages'
    /// </summary>
    public bool MapRazorPagesEndpoints = true;

    /// <summary>
    /// Adds middleware for static files and sets a few default settings:
    /// - allows serving of unknown files types
    /// - compression is set to 'HttpsCompressionMode.Compress' 
    /// - does not append a trailing slash for static files
    /// </summary>
    public bool UseStaticFiles = true;

    /// <summary>
    /// Adds middleware which responds with a exception page
    /// </summary>
    public bool UseExceptionPage = true;

    /// <summary>
    /// Adds middleware for cookie policies
    /// </summary>
    public bool UseHttpsAndSecureCookiePolicy { get; set; } = true;
}
