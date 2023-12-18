# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Web

## First time usage
1. Create a new Asp.Net Core Empty .NET 6 project
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
		var options = new CommonWebApplicationBuilderOptions();
		app.CommonWebApplicationBuilder(options);
	}
	
	public void ConfigureServices(IServiceCollection services)
	{
		var options = new CommonWebApplicationServicesOptions();
		services.CommonWebApplicationServices(options);
	}
}
```

- Now you can run your web application out of the box
- Requests will be mapped to a controller
- .cshtml files will be re-compiled during save 
- Static content such as jpg, png, pdf, js, etc... is allowed to be hosted from your app
- See the docs for the two methods: CommonWebApplicationServices and CommonWebApplicationBuilder for more info

## Package Configurations
* Default and modifiable configurations for this package:

appSettings.json:
```json  
{
	"systemLibraryCommonWeb": {
		"httpBaseClient": {
			"timeoutMilliseconds": 60000,
			"retryRequestTimeoutSeconds": 10,
			"cacheClientConnectionSeconds": 300
		},

		"log": {
			"isEnabled": true,
			"level": "Info" //Pick between: Info, Debug, Warning, Error
		},

		"logMessageBuilder": {
			"appendLoggedInState": true,
			"appendIp": false,
			"appendPath": true,
			"appendBrowser": false,
			"appendCookieInfo": false,
			"format": null
		},

		"cache": {
			"defaultDuration": 180
		}
	}
}
```  
 