using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Put_Plain_Text_Success()
    {
        var bin = new HttpBin();

        var response = bin.Put("Hello world!", MediaType.plain);

        Assert.IsTrue(response.Data.Contains("Hello world!"));
    }

    [TestMethod]
    public void Put_Json_Success()
    {
        var bin = new HttpBin();

        var response = bin.Put("{ \"hello\": \"world\" }", MediaType.plain);

        Assert.IsTrue(response.Data.Contains(": \"world\""));
    }

    [TestMethod]
    public void Put_Poco_As_Json_Success()
    {
        var bin = new HttpBin();

        var car = new Car();
        car.Name = "world";
        var response = bin.Put(car, MediaType.json);

        Assert.IsTrue(response.Data.Contains(": \"world\""));
    }

    [TestMethod]
    public void Put_Anonymous_Object_As_Json_Success()
    {
        var bin = new HttpBin();

        var response = bin.Put(new { name = "world" }, MediaType.json);

        Assert.IsTrue(response.Data.Contains(": \"world\""));
    }

    [TestMethod]
    public void Put_Dynamic_As_Json_Success()
    {
        var bin = new HttpBin();

        var car = new
        {
            name = "world"
        };
        var response = bin.Put(car, MediaType.json);

        Assert.IsTrue(response.Data.Contains(": \"world\""));
    }
}
