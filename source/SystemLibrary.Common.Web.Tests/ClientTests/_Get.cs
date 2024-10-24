using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net.Extensions;

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

    [TestMethod]
    public void Get_Client_Response_Serialize_Data_Success()
    {
        var bin = new HttpBin();
        var response = bin.Get();

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);

        var clientResponse = response as ClientResponse;

        var data = clientResponse.Data;

        Assert.IsTrue(data != null, "Data is null, interface Data explicit implementation changed");
        Assert.IsTrue(data.Json().Length > 10, "Data is too short after serialization");
    }
}
