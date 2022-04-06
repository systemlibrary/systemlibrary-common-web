using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Get_Success()
        {
            var WebService = new HttpBinClient();
            var response = WebService.Get();

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.IsTrue(response.Data.Contains("httpbin.org"));
        }
    }
}
