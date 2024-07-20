using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Web.Tests;

[TestClass]
public partial class ClientTests
{
    [TestMethod]
    public void Get_Success()
    {
        for(int i = 0; i < 20; i++)
        {
            Clock.Measure(() =>
            {
                var client = new Client();

                var response = client.Get<string>("http://httpbin.org/get");

                Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
                Assert.IsTrue(response.Data.Contains("httpbin.org"));
            });
            System.Threading.Thread.Sleep(Randomness.Int(10, 2000));
        }
    }
}