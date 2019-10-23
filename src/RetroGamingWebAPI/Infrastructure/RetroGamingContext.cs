using Microsoft.EntityFrameworkCore;
using RetroGamingWebAPI.Models;

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
            modelBuilder.Entity<Gamer>().ToTable("Gamers")
                .HasMany(x => x.Scores)
                .WithOne()
                .HasForeignKey(x => x.GamerId);

            modelBuilder.Entity<Score>().ToTable("Scores");
        }

        public DbSet<Gamer> Gamers { get; set; }
        public DbSet<Score> Scores { get; set; }
    }

}
