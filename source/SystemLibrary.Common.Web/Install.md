# Installation

## Install nuget package

* Open your project/solution in Visual Studio
* Open Nuget Project Manager
* Search and install SystemLibrary.Common.Web

## First time usage

- Classes and methods can be used out of the box by including the namespace they live in

- Sample:
```csharp  
	public void ConfigureServices(IServiceCollection services)
	{
		services.CommonServices(); //Extension inside this package
	}
```

## Override default configurations
* Example of all default configurations in this package:

appSettings.json:
```json  
	{

		"systemLibraryCommonWeb": {
		}
	}
```  
