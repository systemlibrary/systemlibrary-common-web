# SystemLibrary Common Web

## Requirements
- &gt;= .NET 5
- Microsoft.AspNetCore.App
- Microsoft.Extensions.FileProviders.Physical &gt;= 5.0.0

## Latest Version
- Added option to pass in your own implementation of 'StringOutputFormatter' for media types
- Added media types for pdf, jpg and png
- Redone views and media types registration, the default ones that comes with this library is always registered, and your custom ones are appended

## Description
SystemLibrary.Common.Web for any .NET &gt;= 5 web application - get default settings, routes, etc configured out of the box

Selling points:
* Setup IApplicationBuilder in one line
    * Enables app to use Mvc, Razor and able to serve certain file types (css, js, svg, jpg, png, etc...)
    * Enabled Authorization and Authentication
* Configure IServiceCollection in one line
    * Registers default view locations
    * Registers IHttpContextAccessor as a Service so inject
    * Register MVC, Razor and default ViewLocations
* Extensions for web

## Docs			
Documentation with samples:
https://systemlibrary.github.io/systemlibrary-common-web/

## Nuget
https://www.nuget.org/packages/SystemLibrary.Common.Web/

## Suggestions and feedback
support@systemlibrary.com

## Lisence
- It's free forever, copy paste as you'd like
