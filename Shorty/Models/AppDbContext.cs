using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Link> Links { get; set; }


        // https://metanit.com/sharp/aspnet5/12.1.php
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
