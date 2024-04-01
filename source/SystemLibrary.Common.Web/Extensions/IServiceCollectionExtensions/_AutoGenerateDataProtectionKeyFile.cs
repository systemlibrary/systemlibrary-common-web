using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

partial class IServiceCollectionExtensions
{
    public static IServiceCollection UseAutomaticKeyGenerationFile(this IServiceCollection services, ServicesCollectionOptions options)
    {
        if (!options.UseAutomaticKeyGenerationFile) return services;

        var type = Type.GetType("SystemLibrary.Common.Net.CryptationKeyFile, SystemLibrary.Common.Net");

        if (type == null)
            throw new Exception("SystemLibrary.Common.Net.CryptationKeyFile is not loaded or type is renamed in version you are using");

        var keyFileFullName = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(x => x.Name == "GetKeyFileFullName")
            .FirstOrDefault();

        if (keyFileFullName == null)
            throw new Exception("Method 'GetKeyFileFullName' is renamed or do not exist");

        var baseDirectory = AppContext.BaseDirectory;

        var rootDirectoryInfo = new DirectoryInfo(baseDirectory);

        int searchBinFolderDepth = 4;

        var startSearch = new DirectoryInfo(baseDirectory);
        while (searchBinFolderDepth > 0)
        {
            if (startSearch?.Name?.ToLower() == "bin")
            {
                rootDirectoryInfo = startSearch.Parent;
                break;
            }
            startSearch = startSearch?.Parent;

            searchBinFolderDepth--;
        }

        var keyFullFileName = keyFileFullName.Invoke(null, new object[] { rootDirectoryInfo.FullName }) + "";

        var hasCryptationKeyFile = keyFullFileName.Is();

        var appName = "AppName" + (Assembly.GetEntryAssembly()?.FullName + (hasCryptationKeyFile ? keyFullFileName : baseDirectory)).ReplaceAllWith("-", ",", ".", " ", "=", "/", "\\");

        var builder = services.AddDataProtection();

        if (hasCryptationKeyFile)
        {
            rootDirectoryInfo = new DirectoryInfo(keyFullFileName.Replace(Path.GetFileName(keyFullFileName), ""));

            builder = builder.DisableAutomaticKeyGeneration();
        }

        builder.PersistKeysToFileSystem(rootDirectoryInfo)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 100))
            .SetApplicationName(appName);

        return services;
    }
}