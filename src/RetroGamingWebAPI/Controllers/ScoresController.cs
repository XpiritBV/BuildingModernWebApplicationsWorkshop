using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using RetroGamingWebAPI.Infrastructure;
using RetroGamingWebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RetroGamingWebAPI.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [OpenApiTag("Scores", Description = "API to retrieve or post individual high scores")]
    public class ScoresController : ControllerBase
    {
        private readonly RetroGamingContext context;
        private readonly IMailService mailService;

        public ScoresController(RetroGamingContext context, IMailService mailService)
        {
            this.context = context;
            this.mailService = mailService;
        }

        [MapToApiVersion("2.0")]
        [HttpGet("{game}")]
        public async Task<ActionResult<IEnumerable<Score>>> GetAsync(string game, CancellationToken cancellationToken)
        {
            var scores = await context.Scores
                .Where(s => s.Game == game)
                .Include(s => s.Gamer)
                .ToListAsync(cancellationToken);

            return Ok(scores);
        }

        [HttpPost("{nickname}/{game}")]
        public async Task<ActionResult> PostScore(string nickname, string game, [FromBody] int points, CancellationToken cancellationToken)
        {
            // Lookup gamer based on nickname
            var gamer = await context.Gamers
                  .FirstOrDefaultAsync(g => g.Nickname.ToLower() == nickname.ToLower(), cancellationToken);

            if (gamer == null) return BadRequest();

            // Find highest score for game
            var score = await context.Scores
                  .Where(s => s.Game == game && s.Gamer == gamer)
                  .OrderByDescending(s => s.Points)
                  .FirstOrDefaultAsync(cancellationToken);

            if (score == null)
            {
                score = new Score() { Gamer = gamer, Points = points, Game = game };
                await context.Scores.AddAsync(score, cancellationToken);
            }
            else
            {
                if (score.Points > points)
                    return BadRequest();

                score.Points = points;

            }
            await context.SaveChangesAsync(cancellationToken);

            mailService.SendMail($"New high score of {score.Points} for game '{score.Game}'");

            return Ok();
        }
    }
}