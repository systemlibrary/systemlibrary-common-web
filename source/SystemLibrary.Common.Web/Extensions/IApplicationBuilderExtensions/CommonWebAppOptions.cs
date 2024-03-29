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
///     var options = new CommonWebAppOptions();
///     
///     options.UseHttpRedrectionAndHsts = false;
///     
///     app.CommonWebApplicationBuilder(options);
/// }
/// </code>
/// </example>
public class CommonWebAppOptions
{
    /// <summary>
    /// Adds middleware for http to https redirect
    /// </summary>
    public bool UseHttpToHttpsRedirectionAndHsts = true;

    /// <summary>
    /// Set to true to enable response caching
    /// </summary>
    public bool UseOutputCaching = true;

    public bool UseOutputCacheForAuthenticatedUsers = false;

    /// <summary>
    /// Adds the routing middleware that comes with Aspnet
    /// </summary>
    public bool UseRouting = true;

    /// <summary>
    /// Adds the cookie policy middleware that comes with Aspnet
    /// </summary>
    public bool UseCookiePolicy = true;

    /// <summary>
    /// Adds response compression with Gzip after static file middleware
    /// </summary>
    public bool UseGzipResponseCompression = true;

    /// <summary>
    /// Adds response compression with Brotli after static file middleware
    /// </summary>
    public bool UseBrotliResponseCompression = false;

    /// <summary>
    /// Adds middleware for Authorization and Authentication attributes
    /// </summary>
    public bool UseAuthentication= true;

    /// <summary>
    /// Adds middleware for Authorization attributes
    /// </summary>
    public bool UseAuthorization = true;
    
    /// <summary>
    /// Adds middleware for ForwardHeaders
    /// </summary>
    public bool UseForwardedHeaders = true;

    /// <summary>
    /// Adds middleware to route urls to controllers
    /// </summary>
    public bool UseControllers = true;

    /// <summary>
    /// Adds middleware to route /api/-url's to controllers allowing your APP to have api folder at root
    /// </summary>
    public bool UseApiControllers = true;

    /// <summary>
    /// Adds middleware for razor pages as endpoints aka 'MapRazorPages'
    /// </summary>
    public bool UseRazorPages = true;

    /// <summary>
    /// Adds middleware for static files and sets a few default settings:
    /// - allows serving of unknown files types
    /// - compression is set to 'HttpsCompressionMode.Compress' 
    /// - does not append a trailing slash for static files
    /// </summary>
    public bool UseStaticFiles = true;

    /// <summary>
    /// Set the cache-control max age header to a duration for all static requests
    /// 
    /// Note: requires UseStatisFiles set to True
    /// 
    /// Default: two weeks
    /// </summary>
    public int StaticFilesCacheMaxAgeSeconds = 1209600;

    /// <summary>
    /// Allow serving of unknown, unsupported, media/mime types.
    /// 
    /// Note: requires UseStatisFiles set to True
    /// 
    /// Defaults to false
    /// </summary>
    public bool StaticFileServeUnknownFileTypes = false;

    /// <summary>
    /// Set the relative path of where to only allow serving static content from
    /// 
    /// Note: requires UseStatisFiles set to True
    /// 
    /// For example: /static
    /// </summary>
    public string StaticFileRequestPath = null;

    /// <summary>
    /// Adds middleware which responds with a exception page usually used in development environments and test environments
    /// </summary>
    public bool UseDeveloperPage = true;
}
