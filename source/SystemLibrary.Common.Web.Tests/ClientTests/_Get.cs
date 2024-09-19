using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Get_Success()
    {
        var bin = new HttpBin();
        var response = bin.Get();

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.IsTrue(response.Data.Contains("httpbin.org"));
    }
}
