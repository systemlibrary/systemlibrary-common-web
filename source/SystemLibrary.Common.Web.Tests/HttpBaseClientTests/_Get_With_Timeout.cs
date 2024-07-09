using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Get_With_Large_Timeout_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.GetWithTimeout(13000);

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.IsTrue(response.Data.Contains("httpbin.org"));
        }

        [TestMethod]
        public void Get_With_Short_Timeout_Fails()
        {
            var webService = new HttpBinClient();
            int timeout = 123;

            try
            {
                var response = webService.GetWithTimeout(timeout);

                //Note: Works as intended
                //If all unit tests are ran, the "Large_Timeout_Success()" might run before this one and
                //we will get a cached response as the only difference in the request is the timeout
                Assert.IsTrue(false, "Should have thrown a timeout error");
            }
            catch (TimeoutException ex)
            {
                Dump.Write(ex);
                Assert.IsTrue(ex.Message.Contains(timeout.ToString() + " seconds"));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, ex.Message + " " + ex.GetType().Name);
            }
        }


        [TestMethod]
        public void Get_With_Short_Timeout_Fails_And_Retry_Is_Success()
        {
            var webService = new HttpBinClient(true);
            int timeout = 123;

            try
            {
                var response = webService.GetWithTimeout(timeout);

                //Note: Works as intended
                //If all unit tests are ran, the "Large_Timeout_Success()" might run before this one and
                //we will get a cached response as the only difference in the request is the timeout
                Assert.IsTrue(true);
            }
            catch (RetryHttpRequestException retry)
            {
                Assert.IsTrue(true, "Retry thrown, short timeout, against a long delay request");

            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, "Times out first, then a retry request gives 200 OK, should not have thrown ex: " + ex.ToString());
            }
        }

        [TestMethod]
        public void Get_With_Short_Timeout_Fails_And_Retry_Times_Out()
        {
            var webService = new HttpBinClient(true);
            int timeout = 123;

            try
            {
                var response = webService.GetWithTimeout(timeout);

                //Note: Works as intended
                //If all unit tests are ran, the "Large_Timeout_Success()" might run before this one and
                //we will get a cached response as the only difference in the request is the timeout
                Assert.IsTrue(false, "Should have thrown a timeout error, your timeout on retry is most likely large, so it returns 200 OK.");
            }
            catch (RetryHttpRequestException ex)
            {
                Assert.IsTrue(true, "Retry was not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, ex.ToString());
            }
        }
    }
}
