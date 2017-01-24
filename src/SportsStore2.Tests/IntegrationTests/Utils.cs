using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SportsStore2.API;

namespace SportsStore2.Tests.IntegrationTests
{
    public class Utils
    {
        public static IWebHostBuilder GetHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                       .AddCommandLine(args)
                       .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                       .Build();

            return new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseStartup<Startup>();
        }
    }
}
