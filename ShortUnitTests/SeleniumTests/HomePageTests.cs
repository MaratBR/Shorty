using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ShortUnitTests.SeleniumTests
{
    public class HomePageTests : SeleniumTestsBase
    {
        [Test]
        public void TestHomeHTML()
        {
            Chrome.Navigate().GoToUrl("https://localhost:5001/");
            Thread.Sleep(10000);
            var logo = Chrome.FindElement(By.ClassName("app-title"));
            Assert.NotNull(logo);
        }
    }
}