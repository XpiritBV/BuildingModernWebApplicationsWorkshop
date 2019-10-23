namespace RetroGamingWebAPI.Models
{
    public class Score
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public string Game { get; set; }
        public int GamerId { get; set; }
        public Gamer Gamer { get; set; }
    }
}
