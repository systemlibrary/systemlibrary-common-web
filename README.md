# SystemLibrary Common Web

## Description
A library of classes and methods for any .NET &gt;= 5 web application

* Setup IApplicationBuilder in one line
    * Enables your app to use both Mvc and Razor out of the box
    * Serves a default set of file types (css, js, svg, png, jpg, etc...)
    * Enables Authorization and Authentication
* Configure IServiceCollection in one line
    * Registers default view locations for both Razor and Mvc so it does not matter which one you use
    * Registers IHttpContextAccessor as a service so the interface is injectable
* Extensions for web
* HttpBaseClient (instead of HttpClient from .Net...) to cache underlying TCP connections, so your API calls reuses the same TCP connection for 5 minutes...

## Requirements
- &gt;= .NET 5
- SystemLibrary.Common.Net
- Microsoft.AspNetCore.App
- Microsoft.Extensions.FileProviders.Physical &gt;= 5.0.0

## Latest Version
- Added option to pass in your own implementation of 'StringOutputFormatter' for media types
- Added media types for pdf, jpg and png
- Redone views and media types registration, the default ones that comes with this library is always registered, and your custom ones are appended

## Docs
Documentation with samples:
https://systemlibrary.github.io/systemlibrary-common-web/

## Nuget
https://www.nuget.org/packages/SystemLibrary.Common.Web/

## Suggestions and feedback
support@systemlibrary.com

## Lisence
- It's free forever, copy paste as you'd like