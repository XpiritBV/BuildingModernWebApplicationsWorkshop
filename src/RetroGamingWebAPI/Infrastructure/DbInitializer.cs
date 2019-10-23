using RetroGamingWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RetroGamingWebAPI.Infrastructure
{
    public class DbInitializer
    {
        public static void Initialize(RetroGamingContext context)
        {
            context.Database.EnsureCreated();

            if (context.Gamers.Any()) return;

            context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "LX360", Scores = new List<Score>() { new Score() { Points = 1337, Game = "Pacman" } } });
            context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "LeekGeek", Scores = new List<Score>() { new Score() { Points = 6510, Game = "Space Invaders" } } });

            context.SaveChanges();
        }
    }
}
