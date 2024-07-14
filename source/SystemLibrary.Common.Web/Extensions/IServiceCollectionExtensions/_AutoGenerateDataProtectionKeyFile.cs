using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    internal static string DataProtectionKeyFileName;

    public static IServiceCollection UseAutomaticDataProtectionPolicy(this IServiceCollection services, ServicesCollectionOptions options)
    {
        if (!options.UseAutomaticDataProtectionPolicy) return services;
        
        bool AlreadyRegisteredDataProtectionServices()
        {
            var keyOptions = typeof(IConfigureOptions<KeyManagementOptions>);

            var found = services.FirstOrDefault(sd => sd.ServiceType == keyOptions);

            return found != null;
        }

        if(AlreadyRegisteredDataProtectionServices())
        {
            Log.Warning("UseAutomaticDataProtectionPolicy is set to True, but it seems that data protection is already registered through UseDataProtection().");
            return services;
        }

        var appName = "AppName" +
               Assembly.GetEntryAssembly()?
               .GetName()?
               .Name?
               .ToLower()?
               .ReplaceAllWith("-", ",", ".", " ", "=", "/", "\\")?
               .MaxLength(32);

        var directory = new DirectoryInfo(EnvironmentConfig.Current.ContentRootPath);

        var type = Type.GetType("SystemLibrary.Common.Net.CryptationKey, SystemLibrary.Common.Net");

        if (type == null)
            throw new Exception("SystemLibrary.Common.Net.CryptationKey is not loaded or type is renamed in version you are using");

        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(x => x.Name == "GetKeyFileFullName")
            .FirstOrDefault();

        if (method == null)
            throw new Exception("Method 'GetKeyFileFullName' is renamed or do not exist");

        var keyFileFullName = (string)method.Invoke(null, new object[] { directory.FullName });
        
        var keyFileExists = keyFileFullName.Is();

        if(keyFileExists)
        {
            DataProtectionKeyFileName = Path.GetFileName(keyFileFullName);

            return services.AddDataProtection()
                .DisableAutomaticKeyGeneration()
                .PersistKeysToFileSystem(directory)
                .SetApplicationName(appName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 200))
                .Services;
        }

        return services.AddDataProtection()
                .PersistKeysToFileSystem(directory)
                .SetApplicationName(appName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 200))
                .Services;
    }
}