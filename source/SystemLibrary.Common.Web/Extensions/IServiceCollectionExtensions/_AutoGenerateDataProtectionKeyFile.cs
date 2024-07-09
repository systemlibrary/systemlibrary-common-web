using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    public static IServiceCollection UseAutomaticKeyGenerationFile(this IServiceCollection services, ServicesCollectionOptions options)
    {
        if (!options.UseAutomaticKeyGenerationFile) return services;

        var type = Type.GetType("SystemLibrary.Common.Net.CryptationKey, SystemLibrary.Common.Net");

        if (type == null)
            throw new Exception("SystemLibrary.Common.Net.SystemLibrary is not loaded or type is renamed in version you are using");

        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(x => x.Name == "GetKeyFromDataRingKeyFile")
            .FirstOrDefault();

        if (method == null)
            throw new Exception("Method 'GetKeyFromDataRingKeyFile' is renamed or do not exist");

        var keyFileNameHashed = (string)method.Invoke(null, null);

        var keyFileExists = keyFileNameHashed.Is();

        var appName = "AppName" +
                Assembly.GetEntryAssembly()?
                .GetName()?
                .Name?
                .ToLower()?
                .ReplaceAllWith("-", ",", ".", " ", "=", "/", "\\")?
                .MaxLength(32);

        var builder = services.AddDataProtection();

        var rootDirectoryInfo = new DirectoryInfo(EnvironmentConfig.Current.ContentRootPath);

        if (keyFileExists)
        {
            var dirProperty = type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(x => x.Name == "Dir")
                .FirstOrDefault();

            if (dirProperty == null)
                throw new Exception("Dir property has been renamed or deleted");

            var dir = dirProperty.GetValue(null) + "";

            if(dir.Is())
            {
                rootDirectoryInfo = new DirectoryInfo(dir);
            }

            builder = builder.DisableAutomaticKeyGeneration();
        }

        builder.PersistKeysToFileSystem(rootDirectoryInfo)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 100))
            .SetApplicationName(appName);

        return services;
    }
}