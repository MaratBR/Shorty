using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Shorty;

namespace ShortUnitTests.ApiTests
{
    public abstract class TestServerTestsBase
    {
        protected readonly TestServer Server;
        protected readonly HttpClient Client;

        public TestServerTestsBase()
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
            Server.Host.Start();
            Client = Server.CreateClient();
        }
    }
}