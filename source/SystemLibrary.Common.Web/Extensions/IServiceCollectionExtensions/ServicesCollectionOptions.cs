using System.Reflection;

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
///     var options = new CommonWebServicesOptions();
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
///     app.AddCommonWebServices(options);
/// }
/// </code>
/// </example>
public class ServicesCollectionOptions : BaseOptions
{
    /// <summary>
    /// Set to true to add MVC services
    /// </summary>
    public bool UseMvc = true;

    /// <summary>
    /// Add application assembly as a 'part' so controllers within your application assembly are tried matching against requests
    /// </summary>
    public bool AddApplicationAsPart { get; set; } = true;

    /// <summary>
    /// Add multiple assemblies as a 'part' so controllers within the assemblies are tried matched against requests
    /// </summary>
    public Assembly[] ApplicationParts { get; set; } = null;

     /// <summary>
    /// Enabled re-compilation of .cshtml files upon saving .cshtml files
    /// 
    /// - Avoids the need of a re-compilation of whole application for one small view change
    /// - Package 'System.Security.Cryptography.Pkcs' is not added as dependency, so if you turn this on it will throw exception for a missing package that you must manually add
    /// * Don't want a dependency on that package, as that package is quite large, and this package is also meant for API development
    /// </summary>
    public bool AddRazorRuntimeCompilationOnChange { get; set; } = false;

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
    /// var options = new CommonWebServicesOptions();
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
    /// var options = new CommonWebServicesOptions();
    /// options.ViewLocations = new string[] { "~/Pages/{2}/{1}/{0}.cshtml" }
    /// </code>
    /// </example>
    public string[] AreaViewLocations { get; set; }

    /// <summary>
    /// Create your own class that inherits 'StringOutputFormatter' which sets all 'SupportedMediaTypes' in its constructor
    /// 
    /// A default 'string output formatter' will always be added to your application, so responses/files like CSS, JS, JPG, PNG, JSON, etc are allowed
    /// </summary>
    public StringOutputFormatter AdditionalSupportedMediaTypes { get; set; }

    /// <summary>
    /// Auto-generate a data protection file that will be used for encrypting and decryption data within your application
    /// - string extension methods Encrypt and Decrypt will use the file internally as a key
    /// - cookies read over http will be encrypted and decrypted with the key file, if you host your app over several instances, they must all share the same key of course
    /// </summary>
    public bool UseAutomaticKeyGenerationFile { get; set; } = false;
    
    /// <summary>
    /// Add an internal logger that forwards errors to the ILogWriter of your own choice
    /// - standard output is then forwarded to your own ILogWriter
    /// </summary>
    public bool AddForwardStandardLogging = false;

    public bool UseResponseCaching = true;
}