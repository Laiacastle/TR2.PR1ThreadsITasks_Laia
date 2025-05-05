using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TR2.PR1ThreadsITasks_Laia
{
    public class Palet
    {
        public int Id { get; set; }
        public bool Agafat { get; set; }

        public Palet(int id, bool agafat)
        {
            Agafat = agafat;
            Id = id;
        }
        public override string ToString() => $"Id: {this.Id}, Afagat: {this.Agafat}";
    }
}
