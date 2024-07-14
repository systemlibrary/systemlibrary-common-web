using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web.Tests;

partial class HttpBaseClientTests
{
    [TestMethod]
    public void Post_Multipart_Json_As_Bytes_Return_Partial_Json_Form()
    {
        var data = Assemblies.GetEmbeddedResource("HttpBaseClientTests/Files", "text.json");

        var bytes = data.GetBytes();

        var webService = new HttpBinClient();

        var response = webService.Post(bytes, MediaType.json);

        var form = response.Data.PartialJson<Form>("json/form");

        Assert.IsTrue(form != null, "!form");
        Assert.IsTrue(form.file != null, "!file");
        Assert.IsTrue(form.file.Contains("filename"), "!filename");

        var formstr = response.Data.PartialJson<string>("json/form/file");

        Assert.IsTrue(formstr.Contains("filename"));
    }

    [TestMethod]
    public void Post_Multipart_Return_Partial_Json_Failure()
    {
        var data = Assemblies.GetEmbeddedResource("HttpBaseClientTests/Files", "text.json");

        var bytes = data.GetBytes();

        var webService = new HttpBinClient();

        var response = webService.Post(bytes, MediaType.multipartFormData);

        var form = response.Data.PartialJson<Form>("json/form");

        Assert.IsTrue(form == null, "form exists");
        // NOTE: This does not return 'Form' in the 'json' property, due to the data is sent as 'multipart'
        // hence the response is also 'multipart' and some content length of the bytes sent
        // Specify 'Content-Type' header if you are actually sending json 
    }

    [TestMethod]
    public void Post_Multipart_WithoutHeaders_Returns_MediaType_MultiPart_InResponse_Success()
    {
        var bytes = Assemblies.GetEmbeddedResourceAsBytes("HttpBaseClientTests/Files", "text.json");

        var webService = new HttpBinClient();

        var response = webService.Post(bytes, MediaType.multipartFormData);

        var headersResponse = response.Data.PartialJson<Headers>();

        Assert.IsTrue(headersResponse != null, "!headersResponse");
        Assert.IsTrue(headersResponse.ContentType.Contains("multipart"), "!ContentType");
    }

    [TestMethod]
    public void Post_Multipart_Return_Content_Length_Of_Posted_Bytes_Success()
    {
        var bytes = Assemblies.GetEmbeddedResourceAsBytes("HttpBaseClientTests/Files", "text.json");

        var webService = new HttpBinClient();

        var headers = new Dictionary<string, string>
        {
            { "Content-Type", MediaType.multipartFormData.ToValue() }
        };

        var response = webService.Post(bytes, MediaType.multipartFormData, headers);

        var headersResponse = response.Data.PartialJson<Headers>();

        Assert.IsTrue(headersResponse != null, "!headersResponse");
        Assert.IsTrue(headersResponse.ContentLength > 10, "!ContentLength");
        Assert.IsTrue(headersResponse.ContentType.Contains("multipart"), "!ContentType");
    }

    [TestMethod]
    public void Post_Multipart_Return_Partial_Json_With_Param_CaseInSensitive_Success()
    {
        var bytes = Assemblies.GetEmbeddedResourceAsBytes("HttpBaseClientTests/Files", "text.json");

        var webService = new HttpBinClient();

        var response = webService.Post(bytes, MediaType.multipartFormData);

        var headerResponse = response.Data.PartialJson<Headers>("hEaDerS");


        Assert.IsTrue(headerResponse != null);
        Assert.IsTrue(headerResponse.ContentLength > 1);
    }
}
