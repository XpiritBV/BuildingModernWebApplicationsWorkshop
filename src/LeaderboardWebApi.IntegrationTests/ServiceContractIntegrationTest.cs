using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace LeaderboardWebApi.IntegrationTests
{
    [TestClass]
    public class ServiceContractIntegrationTest
    {
        TestServer testServer;
        HttpClient httpClient;

        [TestInitialize]
        public void Initialize()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseStartup(typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
                .UseEnvironment("IntegrationTest")
                .ConfigureTestServices(services =>
                {
                });

            // Create test stack
            testServer = new TestServer(builder);
            httpClient = testServer.CreateClient();
        }

        [TestMethod]
        public async Task GetReturns200OK()
        {
            // Act
            var response = await httpClient.GetAsync("/api/v1/leaderboard");

            // Assert
            response.EnsureSuccessStatusCode();
            string responseHtml = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseHtml.Contains("1337"));
        }
    }
}
