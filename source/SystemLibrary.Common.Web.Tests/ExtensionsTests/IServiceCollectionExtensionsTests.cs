using System;
using System.IO;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Web.Tests;

[TestClass]
public class IServiceCollectionExtensionsTests
{
    [TestMethod]
    public void Use_Existing_Key_Ring_File_With_Auto_Data_Protection_Policy_True_Gives_Just_Debug_Message()
    {
        var service = Services.Configure();

        service.AddDataProtection()
            .DisableAutomaticKeyGeneration()
            .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Temp\"))
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 1000))
            .SetApplicationName("TestApp");

        var options = new ServicesCollectionOptions();

        options.UseAutomaticDataProtectionPolicy = true;

        service.UseAutomaticDataProtectionPolicy(options);

        Services.Configure(service.BuildServiceProvider());

        var data = "Hello world";
        var enc = data.Encrypt();
        var dec = enc.Decrypt();
        Assert.IsTrue(dec == data, "Wrong decryption");
    }

    [TestMethod]
    public void Use_Existing_Key_Ring_File_With_Auto_Data_Protection_Policy_False_No_Debug_Message()
    {
        var service = Services.Configure();

        service.AddDataProtection()
            .DisableAutomaticKeyGeneration()
            .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Temp\"))
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 1000))
            .SetApplicationName("TestApp");

        var options = new ServicesCollectionOptions();

        options.UseAutomaticDataProtectionPolicy = false;

        service.UseAutomaticDataProtectionPolicy(options);

        Services.Configure(service.BuildServiceProvider());

        var data = "Hello world";
        var enc = data.Encrypt();
        var dec = enc.Decrypt();
        Assert.IsTrue(dec == data, "Wrong decryption");
    }

    [TestMethod]
    public void Use_Auto_Generated_App_Name_As_Key()
    {
        var service = Services.Configure();

        service.AddDataProtection()
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 1000));

        var options = new ServicesCollectionOptions();

        options.UseAutomaticDataProtectionPolicy = true;

        service.UseAutomaticDataProtectionPolicy(options);

        Services.Configure(service.BuildServiceProvider());

        var data = "Hello world";
        var enc = data.Encrypt();
        var dec = enc.Decrypt();
        Assert.IsTrue(dec == data, "Wrong decryption");
    }

}