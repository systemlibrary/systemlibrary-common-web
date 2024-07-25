using System;
using System.Diagnostics;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests;

partial class HttpBaseClientTests
{
    [TestMethod]
    public void Get_Cancel_Callee_Exception_Without_Retrying_Success()
    {
        Stopwatch sw = new Stopwatch();
        sw.Restart();
        sw.Start();
        try
        {
            var service = new HttpBin(false);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            tokenSource.CancelAfter(100);

            var response = service.Get_Retry_Request_Against_Firewall(tokenSource.Token);

            throw new Exception(nameof(service.Get_Retry_Request_Against_Firewall) + " should throw CalleeCancelledRequestException");
        }
        catch (CalleeCancelledRequestException)
        {
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "CalleeCancelledRequestException should be thrown: " + ex.Message + " " + ex.GetType().Name);
        }

        sw.Stop();

        Assert.IsTrue(sw.ElapsedMilliseconds < 1500, "Too long, it did retry when it doesnt have to!");
    }

    [TestMethod]
    public void Get_Cancel_Callee_Exception_With_Retrying_Success()
    {
        try
        {
            var service = new HttpBin(true);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            tokenSource.CancelAfter(100);

            var response = service.Get_Retry_Request_Against_Firewall(tokenSource.Token);

            throw new Exception(nameof(service.Get_Retry_Request_Against_Firewall) + " should throw CalleeCancelledRequestException");
        }
        catch (CalleeCancelledRequestException)
        {
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "CalleeCancelledRequestException should be thrown: " + ex.Message + " " + ex.GetType().Name);
        }
    }
}
