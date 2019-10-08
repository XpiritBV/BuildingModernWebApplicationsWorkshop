using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetroGamingWebApp.Proxy
{
    [Headers("User-Agent: Leaderboard WebAPI Client 1.0")]
    public interface ILeaderboardClient
    {
        [Get("/api/v1.0/leaderboard")]
        Task<IEnumerable<HighScore>> GetHighScores();
    }

    public class HighScore
    {
        public string Game { get; set; }
        public string Nickname { get; set; }
        public int Points { get; set; }
    }
}
