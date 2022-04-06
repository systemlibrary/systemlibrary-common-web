using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Get_Cancel_Request_By_Token_Success()
        {
            var service = new HttpBinClient(false);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            tokenSource.CancelAfter(100);
            try
            {
                var response = service.GetWithCancellationToken(tokenSource.Token);
            }
            catch (CalleeCancelledRequestException)
            {
                Assert.IsTrue(true);
            }
        }
    }
}
