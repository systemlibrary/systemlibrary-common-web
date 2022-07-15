# SystemLibrary Common Web

## Description
A library of classes and methods for any .NET &gt;= 6 web application

* Setup IApplicationBuilder in one line
* Setup CollectionServices in one line
    * Enables your app instantly to serve static files (css, js, png, jpg, etc...)
	* Enables your app instnatly to route requests to your controllers
	* Enables your app with http to https redirection out of the box
	* Enables your app with authorization and authentication middleware out of the box
		* the one that comes with .NET of course, but we enable the middleware for you
	* Enables your app with a few default view locations which you can easily extend
* Contains helpful extension methods for web in general
* Most importantly: Contains HttpBaseClient, no more connection exhaustion on your server, it caches the underlying TCP connection for 5 minutes

## Requirements
- &gt;= .NET 6
- SystemLibrary.Common.Net
- Microsoft.AspNetCore.App
- Microsoft.Extensions.FileProviders.Physical &gt;= 6.0.0

## Latest Version
- Added Cookie Middleware with default turned on http secure and http only
- Updated nuspec dependencies

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