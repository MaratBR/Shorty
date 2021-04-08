using Shorty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Services
{
    public interface ILinksService
    {
        Task<Link> GetOrCreateLink(string fullUrl);

        Task<Link> GetLinkById(string id);

        Task<Link> GetLinkByUrl(string url);

        string NormalizeURL(string url);

        Task<string> GenerateLinkId();
    }
}
