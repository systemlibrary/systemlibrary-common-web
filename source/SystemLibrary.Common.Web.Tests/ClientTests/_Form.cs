using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Form_Posted_Success()
    {
        var client = new Client();

        var data = new HttpBinFormData();
        var url = "https://httpbin.org/post";
        var response = client.Post<string>(url, data, MediaType.xwwwformUrlEncoded);

        Assert.IsTrue(response.IsSuccess, "Not success " + response.Message);
    }

    class HttpBinFormData
    {
        public string comments { get; set; } = "Hello world";
        public string custemail { get; set; } = "test@systemlibrary.com";
        public string custname { get; set; } = "Aa bb";
        public string custtel { get; set; } = "1234567";
        public string delivery { get; set; } = "18:00";
        public string size { get; set; } = "medium";
        public string topping { get; set; } = "bacon";
    }
}

