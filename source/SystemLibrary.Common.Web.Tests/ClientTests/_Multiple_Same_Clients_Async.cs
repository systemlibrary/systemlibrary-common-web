using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests;

partial class ClientTests
{
    [TestMethod]
    [DataRow(8)]
    public void Short_Running_Multiple_Same_Clients_Cached_Async_Success(int repeat)
    {
        Run_Multi_Requests_Async(repeat, new HttpBin(), 0, 0);
    }

    [TestMethod]
    [DataRow(33)]
    public void Short_Running_Multiple_Diff_Clients_Cached_Async_Success(int repeat)
    {
        // This creates multiple HttpClients (against same domain, but they are new wherever timeout and ssl differs)
        Run_Multi_Requests_Async(repeat, new HttpBin(true), 0, 0);

        Run_Multi_Requests_Async(repeat, new HttpBin(false), 0, 0);

        Run_Multi_Requests_Async(repeat, new HttpBin(true, 6565), 0, 0);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 6565, 2);

        Run_Multi_Requests_Async(repeat, new HttpBin(false), 10000, 2);
    }

    [TestMethod]
    [DataRow(3)]
    public void Long_Running_Multiple_No_Clients_Cached_Async_Success(int repeat)
    {
        Run_Multi_Requests_Async(repeat, new HttpBin(true), 0, 0);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 500, 3);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 7000, 3);
        
        Run_Multi_Requests_Async(repeat, new HttpBin(true), 1250, 3);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 11900, 3);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 33000, 3);
    }

    //[TestMethod]
    //[DataRow(33)]
    //public void Very_Long_Running_Multiple_Retry_Clients_Cached_Async_Success(int repeat)
    //{
    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 0, 0);

    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 4500, 3);

    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 12500, 3);

    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 119000, 3);

    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 330000, 3);
    //}

    static object locking = new object();
    static void Run_Multi_Requests_Async(int repeat, HttpBin bin, int longSleep, int sleepIncremental)
    {
        var tasks = new List<Task>();
        var responses = new List<string>();

        var body = "Hello world";

        if (longSleep > 0)
            Thread.Sleep(longSleep);

        for (int i = 0; i < repeat; i++)
        {
            var sleep = i * sleepIncremental;
            tasks.Add(Task.Run(() =>
            {
                if (sleep > 0)
                    Thread.Sleep(sleep);

                var data = bin.PostAsync(body).Result;

                lock (locking)
                {
                    responses.Add("HttpStatusCode " + (int)data?.StatusCode + ", " + data?.Data);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        var temp = responses.ToArray().Where(x => !x.Contains("200")).ToArray();
        var err = string.Join(" ", temp);

        Assert.IsTrue(responses.Count == repeat, "After " + longSleep + " with incremenatal sleep of " + sleepIncremental + " one or more requests were not complete/added, expected " + repeat + " results, but got only " + responses.Count + ": " + err);

        foreach (var result in responses)
        {
            Assert.IsTrue(result.Contains("HttpStatusCode 200"), "One or more inital requests did not return status code 200");
            Assert.IsTrue(result.Contains(body), "Response did not include body message: " + body);
        }
    }
}
