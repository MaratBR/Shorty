using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shorty.Services;
using Shorty.Services.Impl.LinkIdGeneratorService;

namespace ShortUnitTests.Unit
{
    public class LinkIdGenerationTests
    {
        [Test]
        public void TestHashBasedLinkGeneration()
        {
            var generator = new HashIdGeneratorService(new SharedConfiguration {PreferredLinkLength = 6});
            
            Assert.AreEqual("3q584r", generator.GenerateId("http://ya.ru/"));
            Assert.AreEqual("n9egId", generator.GenerateId("http://google.com/"));
            Assert.AreEqual("foms1Q", generator.GenerateId("http://2gis.ru/"));
            
            generator = new HashIdGeneratorService(
                new SharedConfiguration
                {
                    PreferredLinkLength = 6,
                    SecretKey = "secret"
                });
            
            Assert.AreEqual("xAROsg", generator.GenerateId("http://ya.ru/"));
            Assert.AreEqual("5jPIhR", generator.GenerateId("http://google.com/"));
            Assert.AreEqual("m8SYvv", generator.GenerateId("http://2gis.ru/"));
        }

        [Test]
        public void TestRngLinkIdGeneration()
        {
            var preferred = 6;
            var rngGeneration = new RngIdGeneratorService(new SharedConfiguration { PreferredLinkLength = preferred});
            var list = new List<string>();
            
            for (var i = 0; i < 400; i++)
                list.Add(rngGeneration.GenerateId(null)); // normalizedUrl is not used

            var duplicates = list.GroupBy(v => v)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();
            
            Assert.IsEmpty(duplicates, "Found duplicates in list of randomly generated ids");
            Assert.IsEmpty(list.Where(id => id.Length < preferred), 
                $"There are IDs with length less than {preferred}, but PreferredLinkLength is set to {preferred}");
            
            Assert.AreNotEqual(
                rngGeneration.GenerateId("http://url.com/"), 
                rngGeneration.GenerateId("http://url.com/"), 
                "Call to GenerateId for RngIdGeneratorService with the same url gives the same result, but it must be random");
        }
    }
}