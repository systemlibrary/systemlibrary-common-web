using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Head_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.Head(MediaType.json);

            Assert.IsTrue(response.IsSuccess);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }
    }
}
