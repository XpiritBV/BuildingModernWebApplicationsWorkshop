using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroGamingWebAPI.Infrastructure;
using RetroGamingWebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetroGamingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly List<HighScore> scores;
        private readonly RetroGamingContext context;

        public LeaderboardController(RetroGamingContext context)
        {
            this.context = context;
        }

        [HttpGet("{format?}")]
        [FormatFilter]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<HighScore>>> Get()
        {
            var scores = context.Scores
                .Select(score => new HighScore()
                {
                    Game = score.Game,
                    Points = score.Points,
                    Nickname = score.Gamer.Nickname
                });

            return Ok(await scores.ToListAsync().ConfigureAwait(false));
        }
    }
}