using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        [DataRow(3)]
        public void Test_Async_MultipleClients_Cached_Success(int repeat)
        {
            var service = new HttpBinClient();

            var body = "Hello world";

            var tasks = new List<Task>();

            var responses = new List<string>();
            for (int i = 0; i < repeat; i++)
            {
                var sleep = i * 300;
                try
                {
                    tasks.Add(Task.Run(() =>
                    {
                        Thread.Sleep(sleep);

                        var data = service.PostAsync(body).Result;

                        responses.Add("HttpStatusCode " + (int)data.StatusCode + ", " + data.Data);
                    }));
                }
                catch
                {
                    Assert.IsTrue(false, "Error occured!");
                }
            }

            Task.WaitAll(tasks.ToArray());

            Assert.IsTrue(responses.Count == repeat, "One or more requests failed, should have " + repeat + " results, but got only " + responses.Count);
            foreach (var result in responses)
            {
                Assert.IsTrue(result.Contains("HttpStatusCode 200"), "One or more requests did not return status code 200");

                Assert.IsTrue(result.Contains(body), "Response did not include body message: " + body);
            }
        }
    }
}
