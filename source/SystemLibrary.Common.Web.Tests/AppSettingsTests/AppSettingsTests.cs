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
    public void Read_Client_Configurations()
    {
        var clientConfigurations = GetAppSettingsConfiguration("Client");

        Assert.IsTrue(clientConfigurations != null, "Client is null");

        var clientProperties = clientConfigurations.GetType().GetProperties();

        Assert.IsTrue(clientProperties.Count() >= 6, "Too few props in clientProps");
        var count = 0;
        foreach (var property in clientProperties)
        {
            var value = property.GetValue(clientConfigurations)?.ToString();
            if (property.Name.ToLower() == "timeout")
            {
                count++;
                Assert.IsTrue(value == "8686", "timeout is not 8686: " + value);
            }

            if (property.Name.ToLower() == "clientcacheduration")
            {
                count++;
                Assert.IsTrue(value == "60000", "clientcacheduration is not 60000: " + value);
            }

            if (property.Name.ToLower() == "retrytimeout")
            {
                count++;
                Assert.IsTrue(value == "5500", "retrytimeout is not 5500: " + value);
            }

            if (property.Name.ToLower() == "ignoresslerrors")
            {
                count++;
                Assert.IsTrue(value == "True", "ignoreSslErrors is: " + value);
            }

            if (property.Name.ToLower() == "usecircuitbreakerpolicy")
            {
                count++;
                Assert.IsTrue(value == "True", "usecircuitbreakerpolicy is: " + value);
            }

            if (property.Name.ToLower() == "throwonunsuccessful")
            {
                count++;
                Assert.IsTrue(value == "True", "throwonunsuccessful is: " + value);
            }

            if (property.Name.ToLower() == "useretrypolicy")
            {
                count++;
                Assert.IsTrue(value == "True", "useretrypolicy is: " + value);
            }
        }
        Assert.IsTrue(count == 7, "Too few properties found for clientConfig: " + count);

        var cacheConfig = GetAppSettingsConfiguration("cache");

        Assert.IsTrue(cacheConfig != null, "cacheConfig is null");

        var cacheProperties = cacheConfig.GetType().GetProperties();
        count = 0;
        foreach (var property in cacheProperties)
        {
            var value = property.GetValue(cacheConfig)?.ToString();
            if (property.Name.ToLower() == "duration")
            {
                count++;
                Assert.IsTrue(value == "5", "Duration is not 5: " + value);
            }
            if (property.Name.ToLower() == "fallbackduration")
            {
                count++;
                Assert.IsTrue(value == "3", "fallbackduration is not 3: " + value);
            }
        }
        Assert.IsTrue(count == 2, "Too few properties found for cacheConfig: " + count);


        var logConfig = GetAppSettingsConfiguration("log");

        Assert.IsTrue(logConfig != null, "logConfig is null");

        var logProperties = logConfig.GetType().GetProperties();
        count = 0;
        foreach (var property in logProperties)
        {
            var value = property.GetValue(logConfig)?.ToString();

            if (property.Name.ToLower() == "level")
            {
                count++;
                Assert.IsTrue(value == "Debug", "level is not 'Debug', it is " + value);
            }
        }
        Assert.IsTrue(count == 1, "Too few properties found for logConfig: " + count);
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
