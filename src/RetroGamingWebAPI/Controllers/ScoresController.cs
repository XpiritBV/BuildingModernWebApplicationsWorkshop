using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroGamingWebAPI.Infrastructure;
using RetroGamingWebAPI.Models;

namespace RetroGamingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : ControllerBase
    {
        private readonly RetroGamingContext context;
        private readonly IMailService mailService;

        public ScoresController(RetroGamingContext context, IMailService mailService)
        {
            this.context = context;
            this.mailService = mailService;
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

            mailService.SendMail($"New high score of {score.Points} for game '{score.Game}'");
        }
    }
}