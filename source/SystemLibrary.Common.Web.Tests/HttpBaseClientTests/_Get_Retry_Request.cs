using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Get_Retry_Request_Success()
        {
            try
            {
                var service = new HttpBinClient(true);

                var response = service.Get_Retry_Request_Against_Firewall();

                throw new Exception(nameof(service.Get_Retry_Request_Against_Firewall) + " should throw RetryRequestException");
            }
            catch (RetryRequestException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, "RetryRequestException should be thrown: " + ex.Message);
            }
        }
    }
}
