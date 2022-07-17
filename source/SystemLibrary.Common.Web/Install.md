# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Web

## First time usage

- Initialize your web application with a default set of services (classes, injectable) and middlewares (pipeline, classes runs in order they are registered):
 
```csharp 
using SystemLibrary.Common.Web.Extensions;

public class Initialize 
{
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		var options = new WebApplicationBuilderOptions();
		app.CommonWebApplicationBuilder(options);
	}
	
	public void ConfigureServices(IServiceCollection services)
	{
		var options = new ServiceCollectionCommonWebOptions();
		services.CommonWebApplicationServices(options);
	}
}
```

Then inside your program.cs (main method) use the 'Initialize' class
```csharp 
static void main(string[] args) {
	Host.CreateDefaultBuilder(args)
		.ConfigureWebHostDefaults(config =>
		{
			config.UseStartup<Initialize>();
		})
		//other options...
		.Build()
		.Run();
}
```

- After setup your web application is running, ready to host and serve your controllers and files

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
			"level": "Info"
		},

		"logMessageBuilderOptions": {
			"appendLoggedInState": true,
			"appendCurrentPage": true,
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
 