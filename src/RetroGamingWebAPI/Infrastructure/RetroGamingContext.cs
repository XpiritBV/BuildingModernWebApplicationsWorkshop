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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gamer>().ToTable("Gamers");
            modelBuilder.Entity<Score>().ToTable("Scores");
        }

        public DbSet<Gamer> Gamers { get; set; }
        public DbSet<Score> Scores { get; set; }
    }

}
