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
///     var options = new AppBuilderOptions();
///     
///     options.UseHttpRedrectionAndHsts = false;
///     
///     app.CommonWebApplicationBuilder(options);
/// }
/// </code>
/// </example>
public class AppBuilderOptions : BaseOptions
{
    /// <summary>
    /// Adds middleware for http to https redirect client side, aka hsts
    /// </summary>
    public bool UseHsts = true;

    /// <summary>
    /// Adds the routing middleware that comes with Aspnet
    /// </summary>
    public bool UseRouting = true;

    /// <summary>
    /// Adds middleware for Authorization and Authentication attributes
    /// </summary>
    public bool UseAuthentication = true;

    /// <summary>
    /// Adds middleware for Authorization attributes
    /// </summary>
    public bool UseAuthorization = true;

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
    /// Note: requires UseStaticFiles set to True, and the header 'max-age' cannot be added already in the response
    /// 
    /// Default: two weeks
    /// </summary>
    public int StaticFilesMaxAgeSeconds = 1209600;

    /// <summary>
    /// Allow serving of unknown, unsupported, media/mime types.
    /// 
    /// Note: requires UseStatisFiles set to True
    /// 
    /// Defaults to true
    /// </summary>
    public bool StaticFilesServeUnknownFileTypes = true;

    /// <summary>
    /// Set the relative path of where to only allow serving static content from
    /// 
    /// Note: requires UseStatisFiles set to True
    /// 
    /// For example: /static
    /// </summary>
    public string StaticFilesRequestPath = null;

    /// <summary>
    /// Adds middleware which responds with a exception page usually used in development environments and test environments
    /// </summary>
    public bool UseDeveloperPage = true;
}
