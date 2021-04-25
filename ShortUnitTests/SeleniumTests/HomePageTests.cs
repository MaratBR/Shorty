using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ShortUnitTests.SeleniumTests
{
    public class HomePageTests : SeleniumTestsBase
    {
        [Test]
        public void TestLinkInput()
        {
            Chrome.Navigate().GoToUrl("https://localhost:5001/");
            Thread.Sleep(200);
            var btn = Chrome.FindElementByClassName("url-main__btn");
            var input = Chrome.FindElementByClassName("url-main__input");
            
            Assert.IsFalse(btn.Enabled);
            Assert.IsTrue(input.Displayed && btn.Displayed);
            
            input.Click();
            input.SendKeys("http://yandex.ru");
            
            Assert.IsTrue(btn.Enabled);
            btn.Click();
            Thread.Sleep(200);
            Assert.AreNotEqual("http://yandex.ru", input.Text);
        }
        
        [Test]
        public void TestAliasedInput()
        {
            Chrome.Navigate().GoToUrl("https://localhost:5001/");
            Thread.Sleep(200);
            var btn = Chrome.FindElementByClassName("url-main__btn");
            var input = Chrome.FindElementByClassName("url-main__input");
            var typesContainer = Chrome.FindElementByClassName("url-main__type");
            var tags = typesContainer.FindElements(By.TagName("button")).ToList();
            Assert.AreEqual(2, tags.Count);
            Assert.IsTrue(tags[0].GetAttribute("class").Contains("is-primary"));
            Assert.IsFalse(tags[1].GetAttribute("class").Contains("is-primary"));
            
            tags[1].Click();
            Thread.Sleep(100);
            Assert.IsTrue(tags[1].GetAttribute("class").Contains("is-primary"));
            Assert.IsFalse(tags[0].GetAttribute("class").Contains("is-primary"));

            var alias = Chrome.FindElementByClassName("url-main__alias");
            Assert.IsTrue(alias.Displayed);
            
            input.Click();
            input.SendKeys("http://yandex.ru");
            
            Assert.IsFalse(btn.Enabled);
            
            alias.Click();
            var customId = "test" + new Random().Next();
            alias.SendKeys(customId);

            Assert.IsTrue(btn.Enabled);
            
            btn.Click();
            Thread.Sleep(500);
            var browserOrigin = Chrome.ExecuteScript("return location.origin");
            Assert.IsInstanceOf<string>(browserOrigin);
            Assert.AreEqual($"{browserOrigin}/{customId}", input.GetAttribute("value"));
        }

        [Test]
        public void TestHistory()
        {
            Chrome.Navigate().GoToUrl("https://localhost:5001/");
            Thread.Sleep(200);
            var oldCount = Chrome.FindElementsByClassName("sidebar-item__link").Count();
            var btn = Chrome.FindElementByClassName("url-main__btn");
            var input = Chrome.FindElementByClassName("url-main__input");

            input.Click();
            input.SendKeys("http://example.com");
            
            btn.Click();
            Thread.Sleep(200);
            var newCount = Chrome.FindElementsByClassName("sidebar-item__link").Count();
            Assert.AreEqual(oldCount + 1, newCount);
        }

        [Test]
        public void TestInvalidUrl()
        {
            Chrome.Navigate().GoToUrl("https://localhost:5001/");
            var btn = Chrome.FindElementByClassName("url-main__btn");
            var input = Chrome.FindElementByClassName("url-main__input");

            input.Click();
            input.SendKeys("http://e%xample.com");
            
            btn.Click();
            Thread.Sleep(200);

            IWebElement field = null;
            IWebElement body = null;
            
            Assert.DoesNotThrow(() =>
            {
                field = Chrome.FindElementByCssSelector(".errors .error .error__field");
                body = Chrome.FindElementByCssSelector(".errors .error .error__body");
            });
            
            Assert.AreEqual("Link", field.Text);
            Assert.AreEqual("Invalid URI: The hostname could not be parsed.", body.Text);
        }

        [Test]
        public void TestInvalidCustomId()
        {
            Chrome.Navigate().GoToUrl("https://localhost:5001/");
            var btn = Chrome.FindElementByClassName("url-main__btn");
            var input = Chrome.FindElementByClassName("url-main__input");
            var typesContainer = Chrome.FindElementByClassName("url-main__type");
            var tags = typesContainer.FindElements(By.TagName("button")).ToList();
            tags[1].Click();
            Thread.Sleep(100);

            var alias = Chrome.FindElementByClassName("url-main__alias");
            
            alias.Click();
            alias.SendKeys("$$Hello!");
            
            input.Click();
            input.SendKeys("http://example.com");
            
            btn.Click();
            Thread.Sleep(200);

            IWebElement field = null;
            
            Assert.DoesNotThrow(() =>
            {
                field = Chrome.FindElementByCssSelector(".errors .error .error__field");
            });
            
            Assert.AreEqual("CustomId", field.Text);
        }

        [Test]
        public void TestOffline()
        {
            Chrome.Navigate().GoToUrl("https://localhost:5001/");
            Chrome.NetworkConditions = new ChromeNetworkConditions
            {
                IsOffline = true,
                Latency = TimeSpan.FromMilliseconds(1),
                DownloadThroughput = long.MaxValue,
                UploadThroughput = long.MaxValue,
            };

            try
            {
                Thread.Sleep(200);
                var btn = Chrome.FindElementByClassName("url-main__btn");
                var input = Chrome.FindElementByClassName("url-main__input");
                input.Click();
                input.SendKeys("http://yandex.ru");
                btn.Click();
                Thread.Sleep(200);
                
                Assert.DoesNotThrow(() => Chrome.FindElementByCssSelector(".errors .error"));
                Assert.Throws<NoSuchElementException>(() =>
                    Chrome.FindElementByCssSelector(".errors .error .error__field"));
            }
            finally
            {
                Chrome.NetworkConditions = new ChromeNetworkConditions
                {
                    IsOffline = false,
                    Latency = TimeSpan.FromMilliseconds(1),
                    DownloadThroughput = 1_000_000,
                    UploadThroughput = 1_000_000,
                };
            }
        }

        [Test]
        public void TestCopyToClipboard()
        {
            Chrome.Navigate().GoToUrl("https://localhost:5001/");
            var input = Chrome.FindElementByClassName("url-main__input");
            input.Click();
            input.SendKeys("TestText");
            var copy = Chrome.FindElementByClassName("url-main__copy");
            copy.Click();
            var clipboardContent = Chrome.ExecuteScript("return navigator.clipboard.readText()");
            Assert.IsInstanceOf<string>(clipboardContent);
            Assert.AreEqual("TestText", clipboardContent);
        }

        [Test]
        public void TestStats()
        {
            Chrome.Navigate().GoToUrl("https://localhost:5001/");
            Thread.Sleep(200);
            var btn = Chrome.FindElementByClassName("url-main__btn");
            var input = Chrome.FindElementByClassName("url-main__input");
            input.Click();
            input.SendKeys($"http://yandex.ru/{Guid.NewGuid()}");
            btn.Click();
            var countString = Chrome.FindElementByClassName("home__bottom").Text;
            Assert.IsTrue(countString.EndsWith(" links shortened so far"));
            int count = 0;
            
            Assert.DoesNotThrow(() =>
            {
                count = int.Parse(countString.Split(' ')[0]);
            });
            
            Chrome.Navigate().Refresh();
            Thread.Sleep(500);
            
            var countString2 = Chrome.FindElementByClassName("home__bottom").Text;
            Assert.IsTrue(countString2.EndsWith(" links shortened so far"));
            int count2 = 0;
            
            Assert.DoesNotThrow(() =>
            {
                count2 = int.Parse(countString2.Split(' ')[0]);
            });
            Assert.AreEqual(count + 1, count2);
        }
    }
}