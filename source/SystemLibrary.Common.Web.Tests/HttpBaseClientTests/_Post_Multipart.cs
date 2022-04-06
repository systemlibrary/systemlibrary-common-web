using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Post_Multipart_Return_Partial_Json_Success()
        {
            var bytes = Assemblies.GetEmbeddedResourceAsBytes("HttpBaseClientTests/Files", "text.json");

            var webService = new HttpBinClient();
            var response = webService.Post(bytes, MediaType.multipartFormData);

            var form = response.Data.PartialJson<Form>();
            Assert.IsTrue(form.file != null && form.file.Length > 100 && form.file.Contains("emptyObject"));
        }

        [TestMethod]
        public void Post_Multipart_Return_Partial_Json_With_Param_Success()
        {
            var bytes = Assemblies.GetEmbeddedResourceAsBytes("HttpBaseClientTests/Files", "text.json");

            var webService = new HttpBinClient();
            var response = webService.Post(bytes, MediaType.multipartFormData);

            var form = response.Data.PartialJson<Form>("form");
            Assert.IsTrue(form.file != null && form.file.Length > 100 && form.file.Contains("emptyObject"));
        }

        [TestMethod]
        public void Post_Multipart_Return_Partial_Json_With_Param_CaseInSensitive_Success()
        {
            var bytes = Assemblies.GetEmbeddedResourceAsBytes("HttpBaseClientTests/Files", "text.json");

            var webService = new HttpBinClient();
            var response = webService.Post(bytes, MediaType.multipartFormData);

            var form = response.Data.PartialJson<Form>("fORM");
            Assert.IsTrue(form.file != null && form.file.Length > 100 && form.file.Contains("emptyObject"));
        }
    }
}
