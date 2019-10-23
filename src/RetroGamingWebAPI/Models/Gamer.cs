using System;
using System.Collections.Generic;

namespace RetroGamingWebAPI.Models
{
    public class Gamer
    {
        public int Id { get; set; }
        public Guid GamerGuid { get; set; }
        public string Nickname { get; set; }
        public virtual ICollection<Score> Scores { get; set; }
    }
}
