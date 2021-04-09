using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shorty.Models
{
    public class Link
    {
        [Key]
        public string Id { get; set; }
        
        public string UrlHash { get; set; }

        [MaxLength(1000)]
        public string Url { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Hits { get; set; }
    }
}
