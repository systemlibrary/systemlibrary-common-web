# SystemLibrary Common Web

## Description
A library of classes and methods for any .NET &gt;= 6 web application

* Setup IApplicationBuilder in one line: app.CommonWebApplicationBuilder();
* Setup IServiceCollection in one line: services.CommonWebApplicationServices();

The two methods in short enables:
* serving of static common file types (css, js, png, jpg, ...)
* routing requests to controllers
* registers services for AspNet.Mvc
* registers and enabled Authorization and Authentication

Contains simple modules for common tasks in any web application:
* HttpBaseClient - reuses the underlying tcp connection for up to 5 minutes, no more socket exhaustion
* Log - added to global namespace, available from anywhere
  * Log.Write - "equivalent" to console.log in javascript
* Cache - a wrapper over the .NET memory cache with auto-creation of cache keys

## Requirements
- &gt;= .NET 6
- SystemLibrary.Common.Net
- Microsoft.AspNetCore.App
- Microsoft.Extensions.FileProviders.Physical &gt;= 6.0.0

## Latest Version
- 6.0.0.3
- Updated systemLibrary.Common.Net to 6.0.0.2
- Added nuspec dependency on Mvc.Razor.RuntimeCompilation 6.0.7 as 6.0.6 throws error
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