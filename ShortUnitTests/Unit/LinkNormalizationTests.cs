using System.Collections.Generic;
using NUnit.Framework;
using Shorty.Services;
using Shorty.Services.Impl.LinksNormalizationService;

namespace ShortUnitTests.Unit
{
    class LinkNormalizationTests
    {
        private ILinksNormalizationService _normalization;
        
        [SetUp]
        public void Setup()
        {
            _normalization = new UriBuilderNormalizationService();
        }


        private static readonly string[] InvalidUrls = {
            "wrongProtocol://yandex.ru",
            "C:/System/asdasd/test",
            "file:///System/asdasd/test",
            string.Empty,
            "%%%%%yandex.ru",
            "^^^^yandex.ru"
        };

        [Test]
        public void TestInvalidUrl()
        {
            foreach (var invalidUrl in InvalidUrls)
            {
                Assert.Throws<InvalidUrlException>(() => _normalization.NormalizeLink(invalidUrl));
            }
        }
        
        private static readonly Dictionary<string, string> ValidUrls = new Dictionary<string, string>
        {
            {"yandex.ru", "http://yandex.ru/"},
            {"http://yandex.ru", "http://yandex.ru/"},
            {"https://yandex.ru", "https://yandex.ru/"},
            {"https://yandex.ru/", "https://yandex.ru/"},
            {"subdomain.domain.ru", "http://subdomain.domain.ru/"},
            {"http://subdomain.domain.ru", "http://subdomain.domain.ru/"},
            {"https://subdomain.domain.ru", "https://subdomain.domain.ru/"},
            {"http://domain.com:443", "http://domain.com:443/"},
        };
        
        private static readonly Dictionary<string, string> IdnUrls = new Dictionary<string, string>
        {
            // idn
            {"http://россия.рф/", "http://xn--h1alffa9f.xn--p1ai/"},
            {"test.россия.рф/", "http://test.xn--h1alffa9f.xn--p1ai/"},
            {"тест.россия.рф/", "http://xn--e1aybc.xn--h1alffa9f.xn--p1ai/"},
            {"überall-ist.de", "http://xn--berall-ist-8db.de/"},
            {"互联网博物馆.中国", "http://xn--blq79isv7atol8gb207f.xn--fiqs8s/"},
        };

        [Test]
        public void TestCorrectUrl()
        {
            foreach (var validUrl in ValidUrls.Keys)
            {
                Assert.DoesNotThrow(() => _normalization.NormalizeLink(validUrl));
            }
        }

        [Test]
        public void TestNormalizationOutput()
        {
            foreach (var validUrl in ValidUrls)
            {
                var normalized = _normalization.ConvertToString(_normalization.NormalizeLink(validUrl.Key));
                Assert.AreEqual(validUrl.Value, normalized);
            }
        }
        
        [Test]
        public void TestIdnNormalizationOutput()
        {
            foreach (var validUrl in IdnUrls)
            {
                var normalized = _normalization.ConvertToString(_normalization.NormalizeLink(validUrl.Key));
                Assert.AreEqual(validUrl.Value, normalized);
            }
        }
    }
}