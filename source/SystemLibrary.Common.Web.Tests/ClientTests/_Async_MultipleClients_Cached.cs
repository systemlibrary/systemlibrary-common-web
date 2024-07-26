using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests;

partial class ClientTests
{
    [TestMethod]
    [DataRow(6)]
    public void Test_Async_Multiple_Clients_Cached_Success(int repeat)
    {
        var bin = new HttpBin();

        var body = "Hello world";

        var tasks = new List<Task>();

        var responses = new List<string>();
        for (int i = 0; i < repeat; i++)
        {
            var sleep = i * 22;
            try
            {
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(sleep);

                    var data = bin.PostAsync(body).Result;

                    responses.Add("HttpStatusCode " + (int)data.StatusCode + ", " + data.Data);
                }));
            }
            catch
            {
                Assert.IsTrue(false, "Error occured!");
            }
        }

        Task.WaitAll(tasks.ToArray());

        var err = string.Join(" ", responses.ToArray());

        Assert.IsTrue(responses.Count == repeat, "One or more requests failed, should have " + repeat + " results, but got only " + responses.Count + " " + err);
        
        foreach (var result in responses)
        {
            Assert.IsTrue(result.Contains("HttpStatusCode 200"), "One or more inital requests did not return status code 200");

            Assert.IsTrue(result.Contains(body), "Response did not include body message: " + body);
        }

        // NOTE: Was almost 2 minutes, cannot remember why, something with TCP con reused?
        Thread.Sleep(15000 + (repeat * 22));

        responses = new List<string>();
        for (int i = 0; i < repeat; i++)
        {
            var sleep = i * 22;
            try
            {
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(sleep);

                    var data = bin.PostAsync(body).Result;

                    responses.Add("HttpStatusCode " + (int)data.StatusCode + ", " + data.Data);
                }));
            }
            catch
            {
                Assert.IsTrue(false, "Error occured!");
            }
        }

        err = string.Join(" ", responses.ToArray());

        Task.WaitAll(tasks.ToArray());
        Assert.IsTrue(responses.Count == repeat, "One or more requests failed after a sleep of over 3 min (less than 5), should have " + repeat + " results, but got only " + responses.Count + ": " + err);
    }

    [TestMethod]
    [DataRow(25)]
    public void Long_Running_Multiple_Clients_Cached_Async_Success(int repeat)
    {
        var bin = new HttpBin();

        var body = "Hello world";

        var tasks = new List<Task>();

        var responses = new List<string>();
        for (int i = 0; i < repeat; i++)
        {
            var sleep = i * 1;
            try
            {
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(sleep);

                    var data = bin.PostAsync(body).Result;

                    responses.Add("HttpStatusCode " + (int)data.StatusCode + ", " + data.Data);
                }));
            }
            catch
            {
                Assert.IsTrue(false, "Error occured!");
            }
        }

        Task.WaitAll(tasks.ToArray());

        var err = string.Join(" ", responses.ToArray());

        Assert.IsTrue(responses.Count == repeat, "One or more requests failed, should have " + repeat + " results, but got only " + responses.Count + " " + err);

        foreach (var result in responses)
        {
            Assert.IsTrue(result.Contains("HttpStatusCode 200"), "One or more inital requests did not return status code 200");

            Assert.IsTrue(result.Contains(body), "Response did not include body message: " + body);
        }

        // Test sleeping for a long time, almost 2 minutes
        Thread.Sleep(100000 + (repeat * 22));

        responses = new List<string>();
        for (int i = 0; i < repeat; i++)
        {
            var sleep = i * 22;
            try
            {
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(sleep);

                    var data = bin.PostAsync(body).Result;

                    responses.Add("HttpStatusCode " + (int)data.StatusCode + ", " + data.Data);
                }));
            }
            catch
            {
                Assert.IsTrue(false, "Error occured!");
            }
        }

        err = string.Join(" ", responses.ToArray());

        Task.WaitAll(tasks.ToArray());
        Assert.IsTrue(responses.Count == repeat, "One or more requests failed after a sleep of over 3 min (less than 5), should have " + repeat + " results, but got only " + responses.Count + ": " + err);

        // Test sleeping for a long time, over 5 minutes
        Thread.Sleep(320000 + (repeat * 22));

        responses = new List<string>();
        for (int i = 0; i < repeat; i++)
        {
            var sleep = i * 22;
            try
            {
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(sleep);

                    var data = bin.PostAsync(body).Result;

                    responses.Add("HttpStatusCode " + (int)data.StatusCode + ", " + data.Data);
                }));
            }
            catch
            {
                Assert.IsTrue(false, "Error occured!");
            }
        }

        err = string.Join(" ", responses.ToArray());

        Task.WaitAll(tasks.ToArray());
        Assert.IsTrue(responses.Count == repeat, "One or more requests failed after a sleep of over 3 min (less than 5), should have " + repeat + " results, but got only " + responses.Count + ": " + err);
    }
}
