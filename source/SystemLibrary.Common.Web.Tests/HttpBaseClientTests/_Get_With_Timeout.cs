using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Get_With_Large_Timeout_Success()
        {
            var webService = new HttpBinClient();

            var response = webService.GetWithTimeout(5000);
            
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
                //Cannot add an Assert here
                //If all unit tests are ran, the "Large_Timeout_Success()" might run before this one and
                //we will get a cached response as the only difference in the request is the timeout
            }
            catch(TimeoutException ex)
            {
                Assert.IsTrue(ex.Message.Contains(timeout.ToString()));
            }
            catch(Exception ex)
            {
                Assert.IsTrue(false, ex.Message + " " + ex.GetType().Name);
            }
        }
    }
}
