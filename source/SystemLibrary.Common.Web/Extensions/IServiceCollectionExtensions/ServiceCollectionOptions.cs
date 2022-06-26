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
    ///     app.CommonServices(options);
    /// }
    /// </code>
    /// </example>
    public class ServiceCollectionOptions
    {
        public bool AddControllers { get; set; } = true;
        public bool AddMvcPages { get; set; } = true;
        public bool AddRazorPages { get; set; } = true;
        public IViewLocationExpander ViewLocations { get; set; }

        /// <summary>
        /// Create your own class that inherits 'StringOutputFormatter' which sets all 'SupportedMediaTypes' in its constructor,
        /// and pass a new instance of your class as this variable
        /// 
        /// If not set, a default string output formatter will be set
        /// </summary>
        public StringOutputFormatter StringOutputFormatter { get; set; }
    }
}