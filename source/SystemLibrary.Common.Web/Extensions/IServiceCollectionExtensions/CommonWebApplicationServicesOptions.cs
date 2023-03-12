using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
///  Web Application Services Options
/// 
/// All options are 'true' (on) by default
/// </summary>
/// <example>
/// Inside your startup.cs/program.cs...
/// <code>
/// public class CustomViewLocations : IViewLocationExpander
/// {
///     //...implement the interface
///     public IEnumerable&lt;string&gt; ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable&lt;string&gt; viewLocations)
///     {
///         return new string[] {   
///             "~/Folder2/{0}/Index.cshtml"
///         }
///     }
/// }
/// 
/// public void ConfigureServices(IServiceCollection services)
/// {
///     var options = new CommonWebApplicationServicesOptions();
///     
///     options.AddControllers = false;
///     //Note: two ways to add view locations, either through an Expander class
///     options.ViewLocationExpander = new CustomViewLocations();
///     
///     //Or directly adding a string array
///     options.ViewLocations = new string[] {
///         "~/Folder/{0}/Index.cshtml",
///         "~/Folder/{1}/{0}.cshtml"
///     }
///     
///     app.CommonWebApplicationServices(options);
/// }
/// </code>
/// </example>
public class CommonWebApplicationServicesOptions
{
    /// <summary>
    /// Enables MV and also enables razor pages, and then enable default media types output formatters, making your application able to serve: tiff, woff, json, xml, pdf, jpg, png, js, css, and a few other default media types
    /// </summary>
    public bool AddMvc { get; set; } = true;
    /// <summary>
    /// Enables Razor Pages, but if 'AddMvcPages' is true, this setting is ignored, as MVC already enables razor pages
    /// </summary>
    public bool AddRazorPages { get; set; } = true;
    /// <summary>
    /// Enables routing to controllers, if 'AddMvcPages' is true, this setting is ignored, as MVC already enables routing to controllers
    /// </summary>
    public bool AddControllers { get; set; } = true;

    /// <summary>
    /// Sets HttpOnly to Always, Secure as 'SameAsRequest' and MinimumSitePolicy to 'Strict'
    /// </summary>
    public bool AddHttpsAndSecureCookiePolicy { get; set; } = true;

    /// <summary>
    /// Enabled re-compilation of .cshtml files upon saving .cshtml files
    /// 
    /// - Avoids the need of a re-compilation of whole application for one small view change
    /// - Package 'System.Security.Cryptography.Pkcs' is not added as dependency, so if you turn this on it will throw exception for a missing package that you must manually add
    /// * Don't want a dependency on that package, as that package is quite large, and this package is also meant for API development
    /// </summary>
    public bool AddRazorRuntimeReCompilationOnViewChanged { get; set; } = false;

    /// <summary>
    /// Pass in an object that implements the interface if you want to extend View Locations
    /// - Another option is to simply set 'ViewLocations' variable or 'AreaViewLocations'
    /// </summary>
    public IViewLocationExpander ViewLocationExpander { get; set; }

    /// <summary>
    /// Pass in a string array of view location formats
    /// 
    /// Example:
    /// ViewLocations = new string[] { "~/Pages/{1}/{0}.cshtml" };
    /// 
    /// Note: This sets non-area view locations
    /// </summary
    /// <example>
    /// Simple example:
    /// <code>
    /// var options = new CommonWebApplicationServicesOptions();
    /// options.ViewLocations = new string[] { "~/Pages/{2}/{1}/{0}.cshtml" }
    /// </code>
    /// </example>
    public string[] ViewLocations { get; set; }

    /// <summary>
    /// Pass in a string array of area view location formats
    /// 
    /// Example:
    /// AreaViewLocations = new string[] { "~/Area/{2}/{1}/{0}.cshtml" };
    /// 
    /// Note: This sets area view locations
    /// </summary
    /// <example>
    /// Simple example:
    /// <code>
    /// var options = new CommonWebApplicationServicesOptions();
    /// options.ViewLocations = new string[] { "~/Pages/{2}/{1}/{0}.cshtml" }
    /// </code>
    /// </example>
    public string[] AreaViewLocations { get; set; }

    /// <summary>
    /// Create your own class that inherits 'StringOutputFormatter' which sets all 'SupportedMediaTypes' in its constructor
    /// 
    /// A default 'string output formatter' will always be added to your application, so responses/files like CSS, JS, JPG, PNG, JSON, etc are allowed
    /// </summary>
    public StringOutputFormatter SupportedMediaTypes { get; set; }

}