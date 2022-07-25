# SystemLibrary Common Web

## Description
A library of classes and methods for any .NET &gt;= 6 web application

* Setup IApplicationBuilder in one line: app.CommonWebApplicationBuilder();
* Setup IServiceCollection in one line: services.CommonWebApplicationServices();

The two methods in short enables:
* serving of static common file types (css, js, png, jpg, ...)
* routing requests to controllers
* registers services for AspNet.Mvc
* registers and enables the two attributes: Authorization and Authentication

Additionally this package contains modules such as:
* HttpBaseClient - reuses the underlying tcp connection for up to 5 minutes, no more socket exhaustion
* Log - added to global namespace, so Log.Error() is available anywhere
  * Log contains Log.Write(), "equivalent" to console.log in javascript
* Cache - a wrapper over the .NET memory cache with auto-creation of cache keys if you dont need per user cache

## Requirements
- &gt;= .NET 6
- SystemLibrary.Common.Net
- Microsoft.AspNetCore.App Framework
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
- Microsoft.Extensions.FileProviders.Physical &gt;= 6.0.0

## Latest Version
- 6.0.0.7
- Updated systemLibrary.Common.Net to 6.0.0.5
- Added nuspec dependency on Mvc.Razor.RuntimeCompilation 6.0.7 as 6.0.6 throws error
- Throwing error if SupportedMediaTypes are sent as Options to CommonWebApplicationServices() when Mvc and Controllers are not being registered
- SupportedMediaTypes are now registered if AddControllers is true when sent to CommonWebApplicationServices()
- Update docs

## Version history
- View git history of this file if interested

## Docs
Documentation with samples:
https://systemlibrary.github.io/systemlibrary-common-web/

## Nuget
https://www.nuget.org/packages/SystemLibrary.Common.Web/

## Suggestions and feedback
support@systemlibrary.com

## Lisence
- It's free forever, copy paste as you'd like