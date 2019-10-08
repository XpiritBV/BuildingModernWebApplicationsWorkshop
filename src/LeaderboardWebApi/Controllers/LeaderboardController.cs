using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LeaderboardWebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace LeaderboardWebApi.Controllers
{
    public class HighScore
    {
        public string Game { get; set; }
        public string Nickname { get; set; }
        public int Points { get; set; }
    }

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/xml", "application/json")]
    [OpenApiTag("Leaderboard", Description = "New operations that should be only visible for version 3")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(LeaderboardController))]
    public class LeaderboardController : Controller
    {
        private readonly LeaderboardContext context;
        private readonly ILogger<LeaderboardController> logger;

        public LeaderboardController(LeaderboardContext context, ILoggerFactory loggerFactory = null)
        {
            this.context = context;
            this.logger = loggerFactory?.CreateLogger<LeaderboardController>();
        }

        // GET api/leaderboard
        /// <summary>
        /// Retrieve a list of leaderboard scores.
        /// </summary>
        /// <returns>List of high scores per game.</returns>
        /// <response code="200">The list was successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HighScore>), 200)]
        public async Task<ActionResult<IEnumerable<HighScore>>> Get(int limit = 0)
        {
            logger?.LogInformation("Retrieving score list with a limit of {SearchLimit}.", limit);

            var scores = context.Scores
                .Select(score => new HighScore() { 
                    Game = score.Game, 
                    Points = score.Points, 
                    Nickname = score.Gamer.Nickname 
                });

            return Ok(await scores.ToListAsync().ConfigureAwait(false));
        }
    }
}
