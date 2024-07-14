using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Web.Tests;

[TestClass]
public class IServiceCollectionExtensionsTests
{
    [TestMethod]
    public void UseAutomaticDataProtectionPolicy_Ecrypts_And_Decrypts()
    {
        var options = new ServicesCollectionOptions();

        options.UseAutomaticDataProtectionPolicy = true;

        var service = new ServiceCollection();

        Services.Configure(service.UseAutomaticDataProtectionPolicy(options));

        var data = "hello world";

        var enc = data.Encrypt();
        var dec = enc.Decrypt();
        Assert.IsTrue(dec == data && dec != enc, "Wrong: " + dec);

        var enc2 = data.Encrypt();
        var dec2 = enc.Decrypt();
        Assert.IsTrue(dec2 == data && enc2 != enc, "Wrong2: " + dec2);

        var enc3 = data.Encrypt();
        var dec3 = enc.Decrypt();
        Assert.IsTrue(dec3 == data && enc3 != enc2 && enc3 != enc, "Wrong3: " + dec3);
    }

    [TestMethod]
    public void Create_Data_Protection_File()
    {
        var options = new ServicesCollectionOptions();

        options.UseAutomaticDataProtectionPolicy = true;

        var service = new ServiceCollection();

        service.UseAutomaticDataProtectionPolicy(options);
    }

    [TestMethod]
    public void Do_Not_Create_Data_Protection_File()
    {
        var options = new ServicesCollectionOptions();

        options.UseAutomaticDataProtectionPolicy = false;

        var service = new ServiceCollection();

        service.UseAutomaticDataProtectionPolicy(options);
    }
}