﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Post_PlainText_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.Post("hello world", MediaType.plain);

            Assert.IsTrue(response.Data.Contains("hello world"));
        }

        [TestMethod]
        public void Post_UrlEncoded_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.PostUrlEncoded("hello=world&hello2=world2");

            var form = response.Data.PartialJson<Form>();

            Assert.IsTrue(response.Data.Contains("urlencoded"));
            Assert.IsTrue(form != null, "!form posted");
            Assert.IsTrue(form.hello.Is());
        }

        [TestMethod]
        public void Post_Json_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.Post("{ hello:\"world\" }", MediaType.json);

            Assert.IsTrue(response.Data.Contains("world"));
        }

        [TestMethod]
        public void Post_Poco_As_Json_Success()
        {
            var webService = new HttpBinClient();

            var car = new Car();
            car.Name = "world";

            var response = webService.Post(car, MediaType.json);

            Assert.IsTrue(response.Data.Contains("world"));
        }

        [TestMethod]
        public void Post_HttpRequestMessage_Success()
        {

        }

        [TestMethod]
        public void Post_ByteArrayContent_Success()
        {

        }


        [TestMethod]
        public void Post_StringContent_Success()
        {

        }
    }
}
