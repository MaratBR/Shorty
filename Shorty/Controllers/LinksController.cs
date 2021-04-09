using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shorty.Models;
using Shorty.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Shorty.Services.Impl;

namespace Shorty.Controllers
{
    [Route("")]
    [ApiController]
    public class LinksController : ControllerBase
    {
        private readonly ILinksService _linksService;

        public LinksController([FromServices] ILinksService linksService)
        {
            _linksService = linksService;
        }
        
        public class ShortenedLink
        {
            public string LinkId { get; set; }
        }

        public class ShortenLinkRequest
        {
            [Required]
            public string Link { get; set; }
        }

        [HttpPost("shorten")]
        public async Task<ActionResult<ShortenedLink>> ShortenLink(
            [FromBody] ShortenLinkRequest request,
            [FromServices] ILinksService linksService)
        {
            Link link = await linksService.GetOrCreateLink(request.Link);
            return new ShortenedLink { LinkId = link.Id };
        }

        [HttpGet("/{linkId}")]
        public async Task<IActionResult> LinkRedirect(string linkId)
        {
            try
            {
                var link = await _linksService.GetLinkById(linkId);
                return Redirect(link.Url);
            }
            catch (LinkNotFoundException exception)
            {
                return NotFound();
            }
        }
    }
}
