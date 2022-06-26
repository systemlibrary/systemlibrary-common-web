using System;
using System.Linq;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web.Tests
{
    [TestClass]
    public partial class AppSettingsTests
    {
        [TestMethod]
        public void Read_AppSettingsConfiguration()
        {
            var config = GetConfigurationByName("httpBaseClient");

            Assert.IsTrue(config != null, "Current null");

            var properties = config.GetType().GetProperties(); 

            foreach(var property in properties)
            {   
                var value = property.GetValue(config)?.ToString();
                if(property.Name.ToLower() == "timeoutmilliseconds")
                {
                    Assert.IsTrue(value == "20000", "timeoutmilliseconds is not 20.000: " + value);
                }

                if (property.Name.ToLower() == "retryrequesttimeoutseconds")
                {
                    Assert.IsTrue(value == "8", "retryrequesttimeoutseconds is not 8: " + value);
                }

                if (property.Name.ToLower() == "cacheclientconnectionseconds")
                {
                    Assert.IsTrue(value == "100", "cacheclientconnectionseconds is not 100: " + value);
                }
            }
        }

        static object GetConfigurationByName(string systemLibraryCommonNetName)
        {
            var config = GetAppSettingsConfig();
            var configurationProperty = GetAppSettingsConfigPropertyInfo();

            var configuration = configurationProperty.GetValue(config);

            var jsonProperty = configuration.GetType().GetProperties()
                .Where(x => x.Name.ToLower() == systemLibraryCommonNetName.ToLower())
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
}
