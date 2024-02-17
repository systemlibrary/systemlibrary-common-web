# SystemLibrary Common Web

## Description
Library with classes and methods for every &gt;=  .NET 7 web application

#### Features
* Setup IApplicationBuilder and IServiceCollection in one line
  * services.CommonWebApplicationServices();
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
- 7.2.0.1
- Updated deps
- HttpBaseClient deserialization of string to object passed in as Generic, now uses SystemLibrary.Common.Net's default json options for Enum, Long, Type and DateTime conversions

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