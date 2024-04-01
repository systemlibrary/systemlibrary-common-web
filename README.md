# SystemLibrary Common Web

## Description
Library with classes and methods for every &gt;=  .NET 7 web application

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
* HttpBaseClient
  * Reuses the underlying TCP connection for 2 minutes, saving 10's of milliseconds on subsequent requests
  * Retries once if a request fails, by recreating the TCP connection, and trying again with a short (10s) timeout
* Cache
  * Auto-generating cache keys based on variables in "function scope"
  * A 3 minute default cache duration
* HttpContextInstance
  * Current 
* ActionContextInstance
  * Current

## Requirements
- &gt;= .NET 7
- Microsoft.AspNetCore.App Framework

## Latest Version
- 7.3.0.1
- Breaking change: the extenion method for registering common services and middleware are now named: UseCommonWebApp and AddCommonWebServices
- Breaking change: the renamed extension methods, now takes a renamed option object: UseCommonWebApp(AppBuilderOptions) and AddCommonWebServices(ServicesCollectionOptions)
- Breaking change: variables in the two Options arguments are now fields instead of properties. They are also renamed/modified. Fex:UseHttpToHttpsRedirectionAndHsts is split into two, UseHsts and UseHttpsRedirection
- Breaking change: most middlewares and services are 'ON' by default
- Breaking change: middleware order is redone
- New: OutputCache opiton is added and services and middleware is registered if set to True (default: on)
- New: IApplicationBuilder extension method: UseBranch
- StaticFiles: added support for app/javascript, app/xml, text/xml
- New: option to direct "standardlogging" to the "ILogWriter" that you implement, all logs goes to your file if you want. It is off by default.

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