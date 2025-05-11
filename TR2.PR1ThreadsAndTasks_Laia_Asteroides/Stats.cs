using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TR2.PR1ThreadsAndTasks_Laia_Asteroides
{
    public class Stats
    {
        public int Game {get;set;}
        public long SecondsPlayed { get; set; }
        public int AsteroidDodged { get; set; }

        public Stats(int game, long seconds, int asteroids) 
        {
            Game = game;
            SecondsPlayed = seconds;
            AsteroidDodged = asteroids;
        }
    }
}
