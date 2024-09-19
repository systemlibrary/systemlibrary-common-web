# Installation

## Requirements
* &gt;= .NET 7

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search for SystemLibrary.Common.Web
* Install SystemLibrary.Common.Web

## First time usage
1. Create a new Asp.Net Core Empty .NET 7 project
2. Add SystemLibrary.Common.Web nuget package
3. Add Startup.cs at root of the web project
4. Add appSettings.json at root of the web project

```csharp 
using SystemLibrary.Common.Web.Extensions;

public class Startup 
{
	static void Main(string[] args) {
		var host = Host.CreateDefaultBuilder(args);

		host.ConfigureWebHostDefaults(config => {
			config.UseStartup<Startup>();
		})

		//other options...

		host.Buidler().Run();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		var options = new AppBuilderOptions();
		app.UseCommonWebApp(options);
	}
	
	public void ConfigureServices(IServiceCollection services)
	{
		var options = new CommonWebServicesOptions();
		services.AddCommonWebServices(options);
	}
}
```

* Now you can run your web application out of the box
* Requests will be mapped to a controller
* .cshtml files will be re-compiled during save and your site will refresh auto 
* Static content such as jpg, png, pdf, js, etc... is allowed to be hosted from your app
* See the docs for the two methods: AddCommonWebServices and UseCommonWebApp for more info

## Package Configurations
* Below are the default and modifiable configurations for this package

###### appSettings.json:
```json  
{
	"systemLibraryCommonWeb": {
		"debug": false, // Internal debug info in this package is logged if true
		
		"cache": {
			"duration": 180,
			"fallbackDuration": 600,
			"containerSizeLimit": 60000
		},
		
		"client": {
			"timeout": 40001,
			"retryTimeout": 10000,
			"ignoreSslErrors": true,
			"useRetryPolicy": true,
			"throwOnUnsuccessful": true,
			"useRequestBreakerPolicy": false,
			"clientCacheDuration": 1200
		},
		
		"log": {
			"level": "Information", // Trace, Information, Debug, Warning, Error, None
			"appendPath": true,
			"appendLoggedInState": true,
			"appendCorrelationId": true,
			"appendIp": false,
			"appendBrowser": false,
			"appendCookieInfo": false,
			"format": null // "json" or null, null is default
		}
	}
}
```  
 