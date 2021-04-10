using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shorty.Models;
using Shorty.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shorty.Services.Impl.LinksNormalizationService;
using Shorty.Services.Impl.LinksService;

namespace Shorty.Controllers
{
    [Route("")]
    [ApiController]
    public class LinksController : ControllerBase
    {
        private readonly ILinksService _linksService;
        private readonly ILinksNormalizationService _normalization;

        public LinksController(
            [FromServices] ILinksService linksService, 
            [FromServices] ILinksNormalizationService normalization)
        {
            _linksService = linksService;
            _normalization = normalization;
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
            Link link;

            try
            {
                var uri = _normalization.NormalizeLink(request.Link);
                if (uri.Host == HttpContext.Request.Host.Host)
                {
                    var modelState = new ModelStateDictionary();
                    modelState.AddModelError("Link", "You cannot shortify \"shorty\" link");
                    return BadRequest(modelState);
                }

                link = await linksService.GetOrCreateLink(uri);
            }
            catch (InvalidUrlException e)
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError("Link", e.Message);
                return BadRequest(modelState);
            }
            catch (UriFormatException e)
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError("Link", e.Message);
                return BadRequest(modelState);
            }
            
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
