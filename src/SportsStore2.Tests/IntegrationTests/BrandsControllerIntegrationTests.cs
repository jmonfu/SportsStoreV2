using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using NUnit.Framework;
using SportsStore2.API;

namespace SportsStore2.Tests.IntegrationTests
{
    [TestFixture()]
    public class BrandsControllerIntegrationTests
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var projectPath = Path.GetFullPath(Path.Combine(basePath, "../../../../SportsStore2.Tests"));

            var server = new TestServer(GetHostBuilder(new string[] { }).UseContentRoot(projectPath).UseEnvironment("Development").UseStartup<Startup>());
            _client = server.CreateClient();

        }

        private static IWebHostBuilder GetHostBuilder(string[] args)
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

        [Test]
        public async Task Get_ReturnsAListOfBrands_BrandsController()
        {
            var request = "api/Brands/Get";
            var response = await _client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

    }
}
