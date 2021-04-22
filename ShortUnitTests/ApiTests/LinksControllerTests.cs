using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Shorty.Controllers;
using Shorty.Services;

namespace ShortUnitTests.ApiTests
{
    public class LinksControllerTests : ApiTestsBase
    {
        [Test]
        public async Task RedirectNotFound()
        {
            var result = await Client.GetAsync("/this_link_does_not_exists");
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Redirect()
        {
            // shorten
            var link = $"http://example.com/{Guid.NewGuid()}";
            var content = JsonContent.Create(new LinksController.ShortenLinkRequest
            {
                Link = link
            });
            var result = await Client.PostAsync("/shorten", content);
            
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            LinksController.ShortenedLink response = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                response = JsonConvert.DeserializeObject<LinksController.ShortenedLink>(await result.Content.ReadAsStringAsync());
            });
            Assert.NotNull(response);


            result = await Client.GetAsync(response.Id);
            Assert.AreEqual(HttpStatusCode.Redirect, result.StatusCode);
            Assert.AreEqual(link, result.Headers.Location?.ToString());
        }

        [Test]
        public async Task CreateLink()
        {
            var link = $"http://example.com/{Guid.NewGuid()}";
            var content = JsonContent.Create(new LinksController.ShortenLinkRequest
            {
                Link = link
            });
            var result = await Client.PostAsync("/shorten", content);
            
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            LinksController.ShortenedLink response = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                response = JsonConvert.DeserializeObject<LinksController.ShortenedLink>(await result.Content.ReadAsStringAsync());
            });
            Assert.NotNull(response);
            
            // repeat
            
            result = await Client.PostAsync("shorten", content);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            LinksController.ShortenedLink response2 = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                response2 = JsonConvert.DeserializeObject<LinksController.ShortenedLink>(await result.Content.ReadAsStringAsync());
            });
            Assert.NotNull(response2);
            
            Assert.AreEqual(response.Id, response2.Id);
        }
        
        [Test]
        public async Task LinkInfo()
        {
            var link = $"http://example.com/{Guid.NewGuid()}";
            var content = JsonContent.Create(new LinksController.ShortenLinkRequest
            {
                Link = link
            });
            var result = await Client.PostAsync("/shorten", content);
            
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            LinksController.LinkInfo response = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                response = JsonConvert.DeserializeObject<LinksController.LinkInfo>(await result.Content.ReadAsStringAsync());
            });
            Assert.NotNull(response);
            
            // get info
            
            result = await Client.GetAsync($"link-info/{response.Id}");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            LinksController.LinkInfo response2 = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                response2 = JsonConvert.DeserializeObject<LinksController.LinkInfo>(await result.Content.ReadAsStringAsync());
            });
            Assert.NotNull(response2);
            
            Assert.AreEqual(response.Id, response2.Id);
            Assert.AreEqual(0, response2.Hits);

            await Client.GetAsync(response.Id);
            
            // get info again
            result = await Client.GetAsync($"link-info/{response.Id}");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.DoesNotThrowAsync(async () =>
            {
                response2 = JsonConvert.DeserializeObject<LinksController.LinkInfo>(await result.Content.ReadAsStringAsync());
            });
            Assert.NotNull(response);
            Assert.AreEqual(1, response2.Hits);
        }

        [Test]
        public async Task CreateLinkWithCustomId()
        {
            var result = await Client.PostAsync("/shorten", JsonContent.Create(new LinksController.ShortenLinkRequest
            {
                CustomId = "customId",
                Link = "https://ya.ru"
            }));
            result.EnsureSuccessStatusCode();

            result = await Client.GetAsync("customId");
            Assert.AreEqual(HttpStatusCode.Redirect, result.StatusCode);
            Assert.AreEqual("https://ya.ru/", result.Headers.Location?.ToString());
        }
        
        [Test]
        public async Task CreateLinkWithInvalidCustomId()
        {
            var result = await Client.PostAsync("/shorten", JsonContent.Create(new LinksController.ShortenLinkRequest
            {
                CustomId = "#InvalidCustomId",
                Link = "https://ya.ru"
            }));
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [Test]
        public async Task CreateLinkThatAlreadyExists()
        {
            var id = $"CustomId{new Random().Next()}";
            var result = await Client.PostAsync("/shorten", JsonContent.Create(new LinksController.ShortenLinkRequest
            {
                CustomId = id,
                Link = "https://ya.ru"
            }));
            result.EnsureSuccessStatusCode();
            result = await Client.PostAsync("/shorten", JsonContent.Create(new LinksController.ShortenLinkRequest
            {
                CustomId = id,
                Link = "https://ya.ru"
            }));
            Assert.AreEqual(HttpStatusCode.Conflict, result.StatusCode);
        }
        
        [Test]
        public async Task CheckLinksCounter()
        {
            var result = await Client.GetAsync("/stats");
            result.EnsureSuccessStatusCode();

            ILinksService.LinksStats stats = null;
            
            Assert.DoesNotThrowAsync(async () =>
            {
                stats = JsonConvert.DeserializeObject<ILinksService.LinksStats>(await result.Content.ReadAsStringAsync());
            });

            result = await Client.PostAsync("/shorten", JsonContent.Create(new LinksController.ShortenLinkRequest
            {
                Link = $"https://example.ru/{Guid.NewGuid()}" 
            }));
            result.EnsureSuccessStatusCode();
            
            result = await Client.GetAsync("/stats");
            ILinksService.LinksStats stats2 = null;
            
            Assert.DoesNotThrowAsync(async () =>
            {
                stats2 = JsonConvert.DeserializeObject<ILinksService.LinksStats>(await result.Content.ReadAsStringAsync());
            });
            
            Assert.AreEqual(stats.TotalCount + 1, stats2.TotalCount);
            Assert.AreEqual(stats.CountToday + 1, stats2.CountToday);

            
        }
    }
}