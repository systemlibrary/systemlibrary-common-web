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
* Client
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
- Cache.Try now uses "" as default, to auto create cache key, previously was null as param (breaking change)
- Log.IsEnabled package options removed and replaced with new log level "Off" (breaking change)
- HttpBaseClient renamed to Client (breaking change)
    - retries on 502, 504 if HttpMethod is GET, POST or FileRequest (breaking change)
    - UseRetryPolicy: enable to add one additional retry on 502 and 504, and enable one retry on 404 and 500
    - RetryRequestTimeoutSeconds renamed to RetryRequestTimeout and changed from 10s to 10000ms default (breaking change)
    - TimeoutMilliseconds renamed to Timeout, defaults to 40000 down from 60000 (breaking change)
    - Between each retry theres a sleep of 500ms: 40s, 0.5s sleep, 10s retry, 0.5s sleep, 5s retry, ~ total 56s, so it is less than most "proxy/gateway timeouts" of 60 (breaking change)
    - ClientCacheDuration increases to 1200 from 110s
    - retryOnceOnRequestCancelled renamed to useRetryPolicy (breaking change)
    - added option to request break on 20 exceptions in a row for 7 seconds

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