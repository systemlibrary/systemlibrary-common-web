# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Web

## First time usage
- Setup your web application with the most common settings in the AspNet Mvc world targetting .NET >= 6
 
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

- Now your web application is ready to host the most common things in AspNet (controllers, razor views, css files, js files, jpg files, etc...)

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
			"level": "Info/Debug/Warning/Error"
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
 