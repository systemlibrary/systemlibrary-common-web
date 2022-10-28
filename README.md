# SystemLibrary Common Web

## Description
Library with classes and methods for every .NET &gt;= 6 web application

### Features
#### One line setup
- Setup IApplicationBuilder in one line: app.CommonWebApplicationBuilder();
- Setup IServiceCollection in one line: services.CommonWebApplicationServices();

The two methods in short enables:
- serving of static common file types (css, js, png, jpg, ...)
- routes requests to controllers
- registers AspNet.Mvc services
- enables usage of Authorization and Authentication attributes
  
#### Modules
- HttpBaseClient reuses the underlying TCP connection, no more socket exhaustion, saving 10's of milliseconds each subsequent request
- Log class in global namespace
  * Log.Write() is "equivalent" to console.log in javascript  
- Cache class with auto-generating cache keys

## Requirements
- &gt;= .NET 6
- SystemLibrary.Common.Net
- Microsoft.AspNetCore.App Framework
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
- Microsoft.Extensions.FileProviders.Physical &gt;= 6.0.0

## Latest Version
- 6.2.1.5/6
- Added audio mediatypes, video/ogg and a few more default media types to be served "out of the box"
- Log module changes:
  - Does not log additional info like logged in, user IP, cookie info, browser info, if environment is local
  - Stacktrace, UserIp, Browser, Cookie Names are only logged at log level Error
  - Cookie Info is only all cookie names comma seperated, no values, nor the max length of the longest cookie
- A null exception during logging if not all configurations are added to appSettings.json has been fixed
- Updated deps 

#### Version history
- View git history of this file if interested

## Installation
https://systemlibrary.github.io/systemlibrary-common-web/Install.html

## Docs
Documentation with code samples:  
https://systemlibrary.github.io/systemlibrary-common-web/

## Nuget
https://www.nuget.org/packages/SystemLibrary.Common.Web/

## Source
https://github.com/systemlibrary/systemlibrary-common-web

## Suggestions and feedback
support@systemlibrary.com

## Lisence
- Free