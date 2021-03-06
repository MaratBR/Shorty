using Shorty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Services
{
    public interface ILinksService
    {
        Task<Link> GetOrCreateLink(Uri uri);

        Task<Link> CreateLink(Uri uri, string customId);

        Task<Link> GetLinkById(string id);

        Task<Link> GetLinkByUrl(string url);

        Task<Link> IncrementLink(Link link);

        Task DeleteLink(string linkId);

        Task<LinksStats> GetStats();

        public class LinksStats
        {
            public long TotalCount { get; set; }
            
            public long CountToday { get; set; }
        }
    }
}
