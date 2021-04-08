using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Models
{
    public class Link
    {
        public string Id { get; set; }

        public string UrlHash { get; set; }

        public string Url { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Hits { get; set; }
    }
}
