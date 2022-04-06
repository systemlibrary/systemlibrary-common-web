using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Put_PlainText_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.Put("Hello world!", MediaType.textplain);

            Assert.IsTrue(response.Data.Contains("Hello world!"));
        }

        [TestMethod]
        public void Put_Json_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.Put("{ \"hello\": \"world\" }", MediaType.textplain);

            Assert.IsTrue(response.Data.Contains(": \"world\""));
        }

        [TestMethod]
        public void Put_Poco_As_Json_Success()
        {
            var webService = new HttpBinClient();

            var car = new Car();
            car.Name = "world";
            var response = webService.Put(car, MediaType.json);

            Assert.IsTrue(response.Data.Contains(": \"world\""));
        }

        [TestMethod]
        public void Put_AnonymousDynamic_As_Json_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.Put(new { name = "world" }, MediaType.json);

            Assert.IsTrue(response.Data.Contains(": \"world\""));
        }

        [TestMethod]
        public void Put_Dynamic_As_Json_Success()
        {
            var webService = new HttpBinClient();

            var car = new
            {
                name = "world"
            };
            var response = webService.Put(car, MediaType.json);

            Assert.IsTrue(response.Data.Contains(": \"world\""));
        }
    }
}
