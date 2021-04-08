using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shorty.Models;
using Shorty.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinksController : ControllerBase
    {
        public class ShortenedLink
        {
            public string LinkID { get; set; }
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
            return new ShortenedLink { LinkID = link.Id };
        }
    }
}
