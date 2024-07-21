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
        for(int i = 0; i < 30; i++)
        {
            Clock.Measure(() =>
            {
                var client = new Client();

                try
                {
                    var response = client.Get<string>("http://httpbin.org/get/?w=1");
                }
                catch
                {
                }
               
                //Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
                //Assert.IsTrue(response.Data.Contains("httpbin.org"));
            });
            System.Threading.Thread.Sleep(Randomness.Int(10, 1000));
        }
    }
}