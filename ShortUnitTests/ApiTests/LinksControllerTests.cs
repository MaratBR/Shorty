using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Shorty.Controllers;

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


            result = await Client.GetAsync(response.LinkId);
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
            
            Assert.AreEqual(response.LinkId, response2.LinkId);
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

            LinksController.ShortenedLink response = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                response = JsonConvert.DeserializeObject<LinksController.ShortenedLink>(await result.Content.ReadAsStringAsync());
            });
            Assert.NotNull(response);
            
            // get info
            
            result = await Client.GetAsync($"link-info/{response.LinkId}");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            LinksController.LinkInfo response2 = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                response2 = JsonConvert.DeserializeObject<LinksController.LinkInfo>(await result.Content.ReadAsStringAsync());
            });
            Assert.NotNull(response2);
            
            Assert.AreEqual(response.LinkId, response2.Id);
            Assert.AreEqual(0, response2.Hits);

            await Client.GetAsync(response.LinkId);
            
            // get info again
            result = await Client.GetAsync($"link-info/{response.LinkId}");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.DoesNotThrowAsync(async () =>
            {
                response2 = JsonConvert.DeserializeObject<LinksController.LinkInfo>(await result.Content.ReadAsStringAsync());
            });
            Assert.NotNull(response);
            Assert.AreEqual(1, response2.Hits);
            
        }
    }
}