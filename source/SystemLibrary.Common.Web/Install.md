# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Web

## First time usage

- Classes and methods can be used out of the box by including the namespace they live in

- Sample:
```csharp  
	using SystemLibrary.Common.Web.Extensions;
	
	public void ConfigureServices(IServiceCollection services)
	{
		services.CommonWebApplicationServices(); //Extension inside this package
	}
```

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
			}
		}
	}
```  
 