﻿using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Post_Multipart_Json_As_Bytes_Return_Partial_Json_Form()
    {
        var data = Assemblies.GetEmbeddedResource("ClientTests/Files", "text.json");

        var bytes = data.GetBytes();

        var bin = new HttpBin();

        var response = bin.Post(bytes, MediaType.json);

        var form = response.Data.JsonPartial<Form>("json/form");

        Assert.IsTrue(form != null, "!form");
        Assert.IsTrue(form.file != null, "!file");
        Assert.IsTrue(form.file.Contains("filename"), "!filename");

        var formstr = response.Data.JsonPartial<string>("json/form/file");

        Assert.IsTrue(formstr.Contains("filename"));
    }

    [TestMethod]
    public void Post_Multipart_Return_Partial_Json_Failure()
    {
        var data = Assemblies.GetEmbeddedResource("ClientTests/Files", "text.json");

        var bytes = data.GetBytes();

        var bin = new HttpBin();

        var response = bin.Post(bytes, MediaType.multipartFormData);

        var form = response.Data.JsonPartial<Form>("json/form");

        Assert.IsTrue(form == null, "form exists");
        // NOTE: This does not return 'Form' in the 'json' property, due to the data is sent as 'multipart'
        // hence the response is also 'multipart' and some content length of the bytes sent
        // Specify 'Content-Type' header if you are actually sending json 
    }

    [TestMethod]
    public void Post_Multipart_WithoutHeaders_Returns_MediaType_MultiPart_InResponse_Success()
    {
        var bytes = Assemblies.GetEmbeddedResourceAsBytes("ClientTests/Files", "text.json");

        var bin = new HttpBin();

        var response = bin.Post(bytes, MediaType.multipartFormData);

        var headersResponse = response.Data.JsonPartial<Headers>();

        Assert.IsTrue(headersResponse != null, "!headersResponse");
        Assert.IsTrue(headersResponse.ContentType.Contains("multipart"), "!ContentType");
    }

    [TestMethod]
    public void Post_Multipart_Return_Content_Length_Of_Posted_Bytes_Success()
    {
        var bytes = Assemblies.GetEmbeddedResourceAsBytes("ClientTests/Files", "text.json");

        var bin = new HttpBin();

        var headers = new Dictionary<string, string>
        {
            { "Content-Type", MediaType.multipartFormData.ToValue() }
        };

        var response = bin.Post(bytes, MediaType.multipartFormData, headers);

        var headersResponse = response.Data.JsonPartial<Headers>();

        Assert.IsTrue(headersResponse != null, "!headersResponse");
        Assert.IsTrue(headersResponse.ContentLength > 10, "!ContentLength");
        Assert.IsTrue(headersResponse.ContentType.Contains("multipart"), "!ContentType");
    }

    [TestMethod]
    public void Post_Multipart_Return_Partial_Json_With_Param_CaseInSensitive_Success()
    {
        var bytes = Assemblies.GetEmbeddedResourceAsBytes("ClientTests/Files", "text.json");

        var bin = new HttpBin();

        var response = bin.Post(bytes, MediaType.multipartFormData);

        var headerResponse = response.Data.JsonPartial<Headers>("hEaDerS");

        Assert.IsTrue(headerResponse != null);
        Assert.IsTrue(headerResponse.ContentLength > 1);
    }

    [TestMethod]
    public void Post_Gives_Forbidden_With_Message_Part_Of_Response()
    {
        var client = new Client(throwOnUnsuccessful: false);
        var url = "https://some.forbidden.com/test.asp";

        var data = "{\"category\":\"test\",\"calltime\":\"22:22:22\",\"serviceid\":\"a47ec985-c8c4-4118\",\"action\":\"add\",\"phoneNumber\":\"99999974\"}";

        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer 1234" },
        };

        try
        {
            var res = client.Post<string>(url, data, MediaType.json, headers: headers);

            Dump.Write(res);
        }
        catch (Exception e)
        {
            Dump.Write(e);
        }
    }
}
