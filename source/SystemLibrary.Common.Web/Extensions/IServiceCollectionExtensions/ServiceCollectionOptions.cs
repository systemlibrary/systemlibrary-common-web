using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;

namespace SystemLibrary.Common.Web.Extensions
{
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
    ///     var options = new ServiceCollectionOptions();
    ///     options.AddControllers = false;
    ///     options.ViewLocations = new CustomViewLocations();
    ///     app.CommonWebApplicationServices(options);
    /// }
    /// </code>
    /// </example>
    public class ServiceCollectionOptions
    {
        public bool AddControllers { get; set; } = true;
        /// <summary>
        /// Enables MVC and also adds then default media types output formatters, making your application able to serve: tiff, woff, json, xml, pdf, jpg, png, and a few other default media types
        /// </summary>
        public bool AddMvcPages { get; set; } = true;
        public bool AddRazorPages { get; set; } = true;

        /// <summary>
        /// Enables re-compilation of cshtml files upon saving
        /// - avoids the need of re-compilation of whole application if you only change a cshtml file
        /// - Package System.Security.Cryptography.Pkcs is not added as dependency, so if you turn this on; you might need to install that package too
        /// </summary>
        public bool AddRazorRuntimeCompilation { get; set; } = false;

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
        /// Create your own class that inherits 'StringOutputFormatter' which sets all 'SupportedMediaTypes' in its constructor,
        /// and pass a new instance of your class as this variable
        /// 
        /// If not set, a default string output formatter will be set
        /// </summary>
        public StringOutputFormatter StringOutputFormatter { get; set; }
    }
}