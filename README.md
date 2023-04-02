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
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation &gt;= 6
- Microsoft.Extensions.FileProviders.Physical &gt;= 6.0.0

## Latest Version
- 6.8.0.1
- Updated deps and docs


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