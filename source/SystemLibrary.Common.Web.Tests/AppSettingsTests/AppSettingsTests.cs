using System;
using System.Linq;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web.Tests;

[TestClass]
public partial class AppSettingsTests
{
    [TestMethod]
    public void Read_AppSettingsConfiguration()
    {
        var httpBaseClientConfig = GetAppSettingsConfiguration("httpBaseClient");

        Assert.IsTrue(httpBaseClientConfig != null, "httpBaseClientConfig is null");

        var httpBaseClientProperties = httpBaseClientConfig.GetType().GetProperties();

        var count = 0;
        foreach (var property in httpBaseClientProperties)
        {
            var value = property.GetValue(httpBaseClientConfig)?.ToString();
            if (property.Name.ToLower() == "timeoutmilliseconds")
            {
                count++;
                Assert.IsTrue(value == "20000", "timeoutmilliseconds is not 20.000: " + value);
            }

            if (property.Name.ToLower() == "retryrequesttimeoutseconds")
            {
                count++;
                Assert.IsTrue(value == "9", "retryrequesttimeoutseconds is not 9: " + value);
            }

            if (property.Name.ToLower() == "cacheclientconnectionseconds")
            {
                count++;
                Assert.IsTrue(value == "100", "cacheclientconnectionseconds is not 100: " + value);
            }
        }
        Assert.IsTrue(count == 3, "One or more properties were not found in httpBaseClientConfig");

        var cacheConfig = GetAppSettingsConfiguration("cache");

        Assert.IsTrue(cacheConfig != null, "cacheConfig is null");

        var cacheProperties = cacheConfig.GetType().GetProperties();
        count = 0;
        foreach (var property in cacheProperties)
        {
            var value = property.GetValue(cacheConfig)?.ToString();
            if (property.Name.ToLower() == "defaultduration")
            {
                count++;
                Assert.IsTrue(value == "70", "Duration is not 70: " + value);
            }
        }
        Assert.IsTrue(count == 1, "One or more properties were not found in cacheConfig, found count: " + count);


        var logConfig = GetAppSettingsConfiguration("log");

        Assert.IsTrue(logConfig != null, "logConfig is null");

        var logProperties = logConfig.GetType().GetProperties();
        count = 0;
        foreach (var property in logProperties)
        {
            var value = property.GetValue(logConfig)?.ToString();
            if (property.Name.ToLower() == "isenabled")
            {
                count++;
                Assert.IsTrue(value == "True", "isEnabled " + value + ", should be True");
            }

            if (property.Name.ToLower() == "level")
            {
                count++;
                Assert.IsTrue(value == "Debug", "level is not 'Debug', it is " + value);
            }
        }
        Assert.IsTrue(count == 2, "One or more properties were not found in logConfig, found count: " + count);
    }

    static object GetAppSettingsConfiguration(string systemLibraryWebName)
    {
        var config = GetAppSettingsConfig();

        var systemLibraryCommonWebProperty = GetAppSettingsConfigPropertyInfo();

        var configuration = systemLibraryCommonWebProperty.GetValue(config);
        var jsonProperty = configuration.GetType().GetProperties()
            .Where(x => x.Name.ToLower() == systemLibraryWebName.ToLower())
            .FirstOrDefault();

        return jsonProperty.GetValue(configuration);
    }

    static PropertyInfo GetAppSettingsConfigPropertyInfo()
    {
        object config = GetAppSettingsConfig();

        return config.GetType()
           .GetProperties()
           .Where(x => x.Name == "SystemLibraryCommonWeb")
           .FirstOrDefault();
    }

    static object GetAppSettingsConfig()
    {
        var type = Type.GetType("SystemLibrary.Common.Web.AppSettings, SystemLibrary.Common.Web");

        var config = type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == "Current")
            .FirstOrDefault()
            .GetValue(null);

        return config;
    }

    static void ValidateProperties(PropertyInfo[] properties, string errorMessage, string propertyName)
    {
        if (properties == null || properties.Length == 0)
            throw new Exception(errorMessage);

        var names = properties.Select(x => x.Name.ToLower()).ToArray();

        if (!names.Has(propertyName.ToLower()))
            throw new Exception(errorMessage + " Does not contain property: " + propertyName + ". Contains: " + string.Join(" ", names));
    }
}
