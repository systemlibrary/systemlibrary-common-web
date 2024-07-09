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
  * Retries once if a request fails, but this time on a new TCP connection with a limited 10s timeout
* Cache
  * Auto-generating cache keys based on variables in "function scope"
  * A 3 minute default cache duration
* HttpContextInstance
  * Current 
* ActionContextInstance
  * Current
* Services.Get
  * the "service locator"

## Requirements
- &gt;= .NET 7
- Microsoft.AspNetCore.App Framework

## Latest Release Notes
- 7.8.0.1
- Updated deps (top breaking changes: json datetime conversion changed and Config files are read from 'content root', never inside 'bin', etc. Check your app's config/deploy routines and API's dealing with DateTime and JSON
- Cache.TryGet method added
- Log.IsEnabled package options removed and replaced with new log level "Off" (breaking change)
- HttpBaseClient (breaking change)
    - retries on 404, 500, 502, 504 if HttpMethod is GET, POST, HEAD, or OPTION, previously only on GET timeout (breaking change)
    - RetryRequestTimeoutSeconds renamed to RetryRequestTimeoutMs and changed from 10s to 10000ms default (breaking change)
    - TimeoutMilliseconds defaults to 40000 down from 60000 (breaking change)
    - Retries twice, up from one: 40s, 1s sleep, 10s retry, 1s sleep, 5s retry, ~ total 57s, so it is less than most "proxy/gateway timeouts" of 60 (breaking change)
    - CacheClientConnectionSeconds reduced to 110s from 120s as 'other sides' usually ends at 120s
    - retryOnceOnRequestCancelled renamed to useRetryOnErrorPolicy (breaking change)

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