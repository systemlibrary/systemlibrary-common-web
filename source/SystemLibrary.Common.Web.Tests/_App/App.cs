using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Web.Tests._App;

public static class App
{
    public static void Start<TInterface, TImplementation>(string environment = "local")
        where TInterface : class
        where TImplementation : class, TInterface
    {
        var appSettingsPath = AppContext.BaseDirectory + "\\appSettings.json";

        var app = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(config =>
            {
                config.UseEnvironment(environment);
                config.UseStartup<Startup<TInterface, TImplementation>>();
            })
            .ConfigureAppConfiguration(config => config.AddJsonFile(appSettingsPath))
            .Build();

        app.Start();
    }

    class Startup<TInterface, TImplementation>
            where TInterface : class
            where TImplementation : class, TInterface
    {
        public Startup()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommonWebServices();
            services.AddTransient<TInterface, TImplementation>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCommonWebApp(env);
        }
    }
}
