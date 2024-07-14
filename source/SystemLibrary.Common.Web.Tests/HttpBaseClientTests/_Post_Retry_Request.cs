using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests;

partial class HttpBaseClientTests
{
    [TestMethod]
    public void Post_Retry_Request_Fails()
    {
        try
        {
            var service = new HttpBinClient(true);

            var response = service.Post_Retry_Request_Against_Firewall();

            throw new Exception(nameof(service.Post_Retry_Request_Against_Firewall) + " should throw TimeoutException");
        }
        catch (RetryHttpRequestException)
        {
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "Exception thrown should've been a TimeoutException. If it is a RetryException, a RetryException should only occur on GET/OPTION/HEAD HTTPMETHODS: " + ex.Message);
        }
    }

    [TestMethod]
    public void Post_Retry_Request_Fails_Retrying_Skipped()
    {
        try
        {
            var service = new HttpBinClient(false);

            var response = service.Post_Retry_Request_Against_Firewall();

            throw new Exception(nameof(service.Post_Retry_Request_Against_Firewall) + " should throw TimeoutException");
        }
        catch (RetryHttpRequestException ex)
        {
            Assert.IsTrue(false, "Exception thrown should've been a TimeoutException. If it is a RetryException, a RetryException should only occur on GET/OPTION/HEAD HTTPMETHODS: " + ex.Message);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.ToString().Contains("timed out"));
        }
    }
}
