# SystemLibrary Common Web

## Description
Library with classes and methods for every &gt;= .NET 8 web application

#### Features
* Setup IApplicationBuilder and IServiceCollection in one line
  * services.AddCommonWebServices();
    * Registers services for compression, MVC, routing requests to controllers and more...
  * app.CommonWebApplicationBuilder();
    * Enables serving static file types (css, js, png, jpg, ...)
    * Enables compression
    * Maps requests to controllers
    * Enables Authorization and Authentication attributes

#### Modules
* Client
  * Reuses the underlying TCP connection for more than 250 seconds, saving 10's of milliseconds on subsequent requests
  * Recreates the underlying HttpClient every 20 minutes to minimize to transient network failures
  * Retries 502 and 504 status codes once on a new TCP connection
* Cache
  * Auto-generating cache keys based on variables in "function scope"
  * A 3 minute default cache duration
  * A 10 minute fallback cache in case of Get() throws exception
* HttpContextInstance.Current thread safe from within a Http Context
* ActionContextInstance.Current thread safe from within a Action Context

## Requirements
- &gt;= .NET 8
- Microsoft.AspNetCore.App Framework

## Latest Release Notes
- 8.0.1.1
- ClientResponse&lt;T&gt; now inherits ClientResponse with the HttpResponseMessage, to explicit and make it clear which return type a function returns instead of using 'object' or the full generic type (feature)

#### Major Breaking Versions
- 7.8.0.1
- HttpBaseClient rewritten to Client
- Cache.Get auto generate key is triggered on cacheKey "", not null as previously
- Cache.Get always have a fallback, configure it's fallback duration, set to 0 for disabling it
- Log.Level is read from Microsofts 'Logging' in appSettings if not existing in the 'packageConfig'
- Requires minimum SystemLibrary.Common.Net 7.13.0.7
 
#### Version history
- View git history of this file if interested

## Installation
- Simply install the nuget package
- [Installation guide](https://systemlibrary.github.io/systemlibrary-common-web/Install.html)

## Documentation
- [Documentation with code samples](https://systemlibrary.github.io/systemlibrary-common-web/)

## Nuget
- [Nuget package page](https://www.nuget.org/packages/SystemLibrary.Common.Web/)

## Source
- [Github](https://github.com/systemlibrary/systemlibrary-common-web)

## Suggestions and feedback
- [Send us an email](mailto:support@systemlibrary.com)

## License
- Free