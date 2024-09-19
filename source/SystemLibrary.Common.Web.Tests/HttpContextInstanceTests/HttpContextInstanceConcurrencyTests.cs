namespace SystemLibrary.Common.Web.Tests;

using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Web;
using SystemLibrary.Common.Web.Extensions;

[TestClass]
public class HttpContextInstanceConcurrencyTests
{
    private TestServer _server;
    private HttpClient _client;

    [TestInitialize]
    public void Setup()
    {
        _server = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddCommonWebServices();
            })
            .Configure(app =>
            {
                var options = new AppBuilderOptions();
                options.UseHsts = false;
                options.UseHttpsRedirection = false;
                app.UseCommonWebApp(null, options);

                app.Run(async context =>
                {
                    var r = new Random();

                    System.Threading.Thread.Sleep(r.Next(7, maxValue: 33));
                    var username = context.Request.Query["username"];

                    System.Threading.Thread.Sleep(r.Next(7, maxValue: 41));

                    var currentContext = HttpContextInstance.Current;
                    System.Threading.Thread.Sleep(r.Next(7, maxValue: 41));

                    await context.Response.WriteAsync($"{username}|{currentContext?.Request.Query["username"]}");
                });
            }));

        _client = _server.CreateClient();
    }

    [TestMethod]
    public async Task HttpContextInstance_Current_IsThreadSafe_PerRequest()
    {
        var tasks = new Task<string>[5000];

        var r = new Random();
        for (int i = 0; i < tasks.Length; i++)
        {
            var userName = $"User{i}";

            tasks[i] = Task.Run(async () =>
            {
                try
                {
                    System.Threading.Thread.Sleep(r.Next(1, 20));
                    var response = await _client.GetAsync($"?username={userName}");

                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    return default;
                }
            });
        }

        var results = await Task.WhenAll(tasks);

        for (int i = 0; i < results.Length; i++)
        {
            Assert.IsTrue(results[i] == "User" + i + "|" + "User" + i, "Error at " + i + " result is " + results[i]);
        }
    }

}