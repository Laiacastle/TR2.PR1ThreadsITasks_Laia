using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TR2.PR1ThreadsAndTasks_Laia_Asteroides
{
    public class Asteroid: SpaceObject
    {
        public bool IsFinalTouched { get; set; }

        public Asteroid(int x, int y) : base(x, y) { IsFinalTouched = false; }
    }
}
