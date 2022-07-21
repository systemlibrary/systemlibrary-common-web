# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Web

## First time usage
1. Create a new empty .NET 6 project
2. Add SystemLibrary.Common.Web
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

- Now you can run your web application out of the box (mapping requests to controllers, .cshtml compilation, serving static files like 'css', 'js', 'jpg', etc...)

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

		"logMessageBuilderOptions": {
			"appendLoggedInState": true,
			"appendCurrentUrl": true,
			"appendIp": true,
			"appendBrowser": true,
			"appendCookieInfo": true
		},

		"cache": {
			"defaultDuration": 180
		}
	}
}
```  
 