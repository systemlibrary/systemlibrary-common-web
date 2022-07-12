using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void Get_With_CacheDuration_Success()
        {
            var WebService = new HttpBinClient();

            var dict = new ConcurrentDictionary<string, string>();

            var tasks = new List<Task>();

            void Add(ClientResponse<string> response, int i)
            {
                dict.TryAdd(i + "", response.Data);
            }

            tasks.Add(Task.Run(() => Add(WebService.Get(), 1)));
            tasks.Add(Task.Run(() => Add(WebService.Get(), 2)));
            tasks.Add(Task.Run(() => Add(WebService.Get(), 3)));
            tasks.Add(Task.Run(() => Add(WebService.Get(),4 )));

            var task = Task.WhenAll(tasks.ToArray());

            task.ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsTrue(dict.Count == 4, "Dictionary does not contain 4 responses");
        }
    }
}
