using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;

namespace SystemLibrary.Common.Web.Extensions;

/// <summary>
/// Options class for Service registration
/// </summary>
/// <example>
/// Inside your 'Startup' class:
/// <code>
/// //Add your custom view locations, where your app has views located
/// public class CustomViewLocations : IViewLocationExpander
/// {
///    //...implement the interface
/// }
/// 
/// //the familiar 'startup.cs':
/// public void ConfigureServices(IServiceCollection services)
/// {
///     var options = new CommonWebApplicationServicesOptions();
///     options.AddControllers = false;
///     options.ViewLocations = new CustomViewLocations();
///     app.CommonWebApplicationServices(options);
/// }
/// </code>
/// </example>
public class CommonWebApplicationServicesOptions
{
    /// <summary>
    /// Enables MVC and also adds then default media types output formatters, making your application able to serve: tiff, woff, json, xml, pdf, jpg, png, and a few other default media types
    /// </summary>
    public bool AddMvcPages { get; set; } = true;
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
    /// - Another option is to simply set 'ViewLocations' variable
    /// </summary>
    public IViewLocationExpander ViewLocationExpander { get; set; }

    /// <summary>
    /// Pass in a string array of view location formats, for instance:
    /// ViewLocations = new string[] { "~/Pages/{0}/{1}.cshtml" };
    /// </summary>
    public string[] ViewLocations { get; set; }

    /// <summary>
    /// Create your own class that inherits 'StringOutputFormatter' which sets all 'SupportedMediaTypes' in its constructor
    /// 
    /// A default 'string output formatter' will always be added to your application, so responses/files like CSS, JS, JPG, PNG, JSON, etc are allowed
    /// </summary>
    public StringOutputFormatter SupportedMediaTypes { get; set; }
    
}