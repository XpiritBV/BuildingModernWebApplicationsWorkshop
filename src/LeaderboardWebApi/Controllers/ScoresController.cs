using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderboardWebApi.Infrastructure;
using LeaderboardWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LeaderboardWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ScoresController : Controller
    {
        private readonly LeaderboardContext context;

        public ScoresController(LeaderboardContext context)
        {
            this.context = context;
        }

        [HttpGet("{game}")]
        public async Task<IEnumerable<Score>> Get(string game)
        {
            var scores = context.Scores.Where(s => s.Game == game).Include(s => s.Gamer);
            return await scores.ToListAsync().ConfigureAwait(false);
        }

        [HttpPost("{nickname}/{game}")]
        public async Task PostScore(string nickname, string game, [FromBody] int points)
        {
            // Lookup gamer based on nickname
            Gamer gamer = await context.Gamers
                .FirstOrDefaultAsync(g => g.Nickname.ToLower() == nickname.ToLower())
                .ConfigureAwait(false);

            if (gamer == null) return;

            // Find highest score for game
            var score = await context.Scores
                .Where(s => s.Game == game && s.Gamer == gamer)
                .OrderByDescending(s => s.Points)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (score == null)
            {
                score = new Score() { Gamer = gamer, Points = points, Game = game };
                await context.Scores.AddAsync(score);
            }
            else
            {
                if (score.Points > points) return;
                score.Points = points;
            }
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
