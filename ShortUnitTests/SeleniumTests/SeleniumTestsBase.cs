using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using ShortUnitTests.ApiTests;
using Shorty;

namespace ShortUnitTests.SeleniumTests
{
    public class SeleniumTestsBase
    {
        protected ChromeDriver Chrome;
        private Process _process;

        [OneTimeSetUp]
        public void SetUpSelenium()
        {
            Chrome = new ChromeDriver(".");
        }

        [OneTimeSetUp]
        public void RunServer()
        {
            var cwd = Directory.GetCurrentDirectory();
            var shortyRoot = Directory.GetParent(cwd).Parent.Parent.Parent.ToString();
            shortyRoot = Path.Join(shortyRoot, "Shorty");
            _process = Process.Start(new ProcessStartInfo
            {
                FileName = "Shorty.exe",
                WorkingDirectory = shortyRoot
            });
        }

        [OneTimeTearDown]
        public void DisposeSelenium()
        {
            Chrome?.Quit();
            Chrome?.Dispose();
            Chrome = null;
        }

        [OneTimeTearDown]
        public void StopServer()
        {
            _process.Kill();
        }
        
        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}