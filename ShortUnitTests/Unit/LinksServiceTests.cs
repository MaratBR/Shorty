﻿using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shorty.Models;
using Shorty.Services;
using Shorty.Services.Impl.LinkIdGeneratorService;
using Shorty.Services.Impl.LinksNormalizationService;
using Shorty.Services.Impl.LinksService;

namespace ShortUnitTests.Unit
{
    public class LinksServiceTests : DbTestsBase
    {
        private readonly ILinksService _linksService;
        
        public LinksServiceTests()
        {
            var shared = new SharedConfiguration();
            var generator = new RngIdGeneratorService(shared);
            _linksService = new EntityLinksService(DbContext, generator, new UriBuilderNormalizationService());
        }

        [Test]
        public void GetNonExistingLink()
        {
            Assert.ThrowsAsync<LinkNotFoundException>(
                async () =>
                {
                    await _linksService.GetLinkById("this_id_does_not_exists");
                });
            
            
            Assert.ThrowsAsync<LinkNotFoundException>(
                async () =>
                {
                    await _linksService.GetLinkByUrl("https://this_id_does_not_exists.com");
                });
        }

        [Test]
        public void CreateLink()
        {
            var uri = new Uri($"http://example.com/{Guid.NewGuid()}");
            Link link = null;
            
            Assert.DoesNotThrowAsync(
                async () =>
                {
                    link = await _linksService.GetOrCreateLink(uri);
                });
            
            Assert.NotNull(link);
            Assert.NotNull(link.Id);
            Assert.LessOrEqual((DateTime.UtcNow - link.CreatedAt).Seconds, 60,
                "Link's creation date is more than 60 seconds smaller than it should");

            Link link2 = null;
            
            Assert.DoesNotThrowAsync(
                async () =>
                {
                    link2 = await _linksService.GetLinkByUrl(uri.ToString());
                });
            Assert.NotNull(link2);
            Assert.AreEqual(link.Id, link2.Id);
            Assert.AreEqual(link.UrlHash, link2.UrlHash);
            
            
            Link link3 = null;
            
            Assert.DoesNotThrowAsync(
                async () =>
                {
                    link3 = await _linksService.GetLinkById(link.Id);
                });
            Assert.NotNull(link2);
            Assert.AreEqual(link.Id, link3.Id);
            Assert.AreEqual(link.UrlHash, link3.UrlHash);
        }
    }
}