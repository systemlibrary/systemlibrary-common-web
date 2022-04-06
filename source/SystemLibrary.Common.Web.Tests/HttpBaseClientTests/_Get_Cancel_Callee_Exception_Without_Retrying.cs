using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Get_Cancel_Callee_Exception_Without_Retrying_Success()
        {
            try
            {
                var service = new HttpBinClient(true);

                CancellationTokenSource tokenSource = new CancellationTokenSource();

                tokenSource.CancelAfter(100);

                var response = service.Get_Retry_Request_Against_Firewall(tokenSource.Token);

                throw new Exception(nameof(service.Get_Retry_Request_Against_Firewall) + " should throw CalleeCancelledRequestException");
            }
            catch (CalleeCancelledRequestException)
            {
                Assert.IsTrue(true);
            }
            catch(Exception ex)
            {
                Assert.IsTrue(false, "CalleeCancelledRequestException should be thrown: "  + ex.Message);
            }
        }
    }
}
