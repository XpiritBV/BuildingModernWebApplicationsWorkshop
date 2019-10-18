using Microsoft.EntityFrameworkCore;
using RetroGamingWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetroGamingWebAPI.Infrastructure
{
    public class RetroGamingContext : DbContext
    {
        public RetroGamingContext(DbContextOptions<RetroGamingContext> options)
            : base(options)
        {
        }

        public DbSet<Gamer> Gamers { get; set; }
        public DbSet<Score> Scores { get; set; }
    }

}
