using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Shorty;

namespace ShortUnitTests.ApiTests
{
    public abstract class ApiTestsBase
    {
        protected readonly TestServer Server;
        protected readonly HttpClient Client;

        public ApiTestsBase()
        {
            Server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(
                    (context, builder) =>
                    {
                        builder
                            .AddJsonFile("appsettings.json")
                            .AddJsonFile("appsettings.Testing.json")
                            .AddEnvironmentVariables();
                    })                
                .UseStartup<Startup>());
            Client = Server.CreateClient();
        }
    }
}