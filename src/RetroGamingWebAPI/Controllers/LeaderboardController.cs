using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using RetroGamingWebAPI.Infrastructure;
using RetroGamingWebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetroGamingWebAPI.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [OpenApiTag("Leaderboard", Description = "API to retrieve high score leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private readonly RetroGamingContext context;

        public LeaderboardController(RetroGamingContext context)
        {
            this.context = context;
        }

        // GET api/leaderboard
        /// <summary>
        /// Retrieve a list of leaderboard scores.
        /// </summary>
        /// <returns>List of high scores per game.</returns>
        /// <response code="200">The list was successfully retrieved.</response>
        [ProducesResponseType(typeof(IEnumerable<HighScore>), 200)]
        [HttpGet("{format?}")]
        [FormatFilter]
        [Produces("application/json")]
        [MapToApiVersion("1.0")]
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

        // GET api/leaderboard
        /// <summary>
        /// Retrieve a list of leaderboard scores.
        /// </summary>
        /// <returns>List of high scores per game.</returns>
        /// <response code="200">The list was successfully retrieved.</response>
        [ProducesResponseType(typeof(IEnumerable<HighScore>), 200)]
        [HttpGet("{format?}")]
        [FormatFilter]
        [Produces("application/json", "application/xml")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<IEnumerable<HighScore>>> GetV2(int limit)
        {
            var scores = context.Scores
                .Select(score => new HighScore()
                {
                    Game = score.Game,
                    Points = score.Points,
                    Nickname = score.Gamer.Nickname
                }).Take(limit);

            return Ok(await scores.ToListAsync().ConfigureAwait(false));
        }
    }
}