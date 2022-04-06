using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Delete_Json_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.Delete("{ \"hello\": \"world\"}", MediaType.json);

            Assert.IsTrue(response.Data.Contains(": \"world\""));
        }
    }
}
