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
            public string Id { get; set; }

            public string Addr { get; set; }
        }

        public class ShortenLinkRequest
        {
            [Required]
            public string Link { get; set; }
            
            
            public string CustomId { get; set; }
        }

        [HttpPost("shorten")]
        public async Task<ActionResult<ShortenedLink>> ShortenLink(
            [FromBody] ShortenLinkRequest request)
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

                if (request.CustomId == null)
                {
                    link = await _linksService.GetOrCreateLink(uri);
                }
                else
                {
                    link = await _linksService.CreateLink(uri, request.CustomId);
                }
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
            catch (InvalidIdException e)
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError("CustomId", e.Message);
                return BadRequest(modelState);
            }
            
            return new ShortenedLink
            {
                Id = link.Id,
                Addr = link.Url
            };
        }

        [HttpGet("/{linkId}")]
        public async Task<IActionResult> LinkRedirect(string linkId)
        {
            try
            {
                var link = await _linksService.GetLinkById(linkId);
                await _linksService.IncrementLink(link);
                return Redirect(link.Url);
            }
            catch (LinkNotFoundException exception)
            {
                return NotFound();
            }
        }

        public class LinkInfo
        {
            public string Id { get; set; }

            public string Addr { get; set; }
            
            public long Hits { get; set; }
            
            public long Ts { get; set; }
        }

        [HttpGet("/link-info/{linkId}")]
        public async Task<ActionResult<LinkInfo>> GetInfo(string linkId)
        {
            try
            {
                var link = await _linksService.GetLinkById(linkId);

                return new LinkInfo
                {
                    Hits = link.Hits,
                    Addr = link.Url,
                    Id = link.Id,
                    Ts = ((DateTimeOffset)link.CreatedAt).ToUnixTimeSeconds()
                };
            }
            catch (LinkNotFoundException e)
            {
                return NotFound($"Link {linkId} not found");
            }
        }
    }
}
