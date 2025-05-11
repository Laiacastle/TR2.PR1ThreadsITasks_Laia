using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TR2.PR1ThreadsAndTasks_Laia_Asteroides
{
    public class Ship
    {
        public int PosX {get;set;}
        public int PosY {get;set;}
        public int Lives { get; set; }
        
        public Ship(int x, int y)
        {
            PosX = x;
            PosY = y;
            Lives = 3;
        }
    }
}
