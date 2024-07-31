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
- Cache.TryGet method added (new)
- TypeDescriptor for string to Enum added, uses ToEnum from SystemLibrary.Common.Net (new)
- Package configurations have changed: https://systemlibrary.github.io/systemlibrary-common-web/Install.html (breaking change)
- Cache.Get now uses "" as default, to auto create cache key, previously was null as param (breaking change)
- Cache.Get always have a fallback, configure it's default fallback duration (600s), set to 0 for disabling it (breaking change)
- Cache containerSizeLimit option added to appSettings (new)
- Log Level "None" added (new)
- Log/ILogWriter IsEnabled option removed (breaking change)
- Log.Info renamed to "Information" (breaking change)
- Log.Level is read appSettings specific config, else from 'Logging', if still not found? Defaults to Warning, up from Info (breaking change)
- HttpBaseClient rewritten to Client (breaking change)
    - Hopefully: just rename HttpBaseClient to Client and the update for this Class should be working, but here are some details:
    - retries on 502, 504 if HttpMethod is GET, POST or FileRequest (breaking change)
    - UseRetryPolicy: enable to add one additional retry on 502 and 504, and enable one retry on 404 and 500
    - RetryRequestTimeoutSeconds renamed to RetryTimeout and changed from 10s to 10000ms default (breaking change)
    - TimeoutMilliseconds renamed to Timeout, defaults to 40001 down from 60000 (breaking change)
    - Total default timeout was 60 seconds, now it is ~56s by default, as most proxies/gateways has that as a default (breaking change)
    - ClientCacheDuration (HttpClient cache) increased to 1200 from 110s
    - retryOnceOnRequestCancelled renamed to useRetryPolicy (breaking change)
    - added option to request break on 20 exceptions in a row for 7 seconds
- Updated dep SystemLibrary.Common.Net which has breaking changes in it (breaking change)
    - Encrypt() and Decrypt() without Key/IV, but using the built-in or configured key/IV, so using Encrypt/Decrypt without params? Please update by decrypt on previous version, and re-encrypt on latest version
    - Json string to DateTime rewritten, so API's regarding Dates, please test

#### Major Breaking Versions
- 7.8.0.1
- HttpBaseClient rewritten to Client
- Cache.Get auto generate key is triggered on cacheKey "", not null as previously
- Cache.Get always have a fallback, configure it's fallback duration, set to 0 for disabling it
- Log.Level is read from Microsofts 'Logging' in appSettings if not existing
- Requires minimum SystemLibrary.Common.Net 7.13.0.7
 
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