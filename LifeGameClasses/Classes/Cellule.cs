using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGameClasses.Classes
{
    public class Cellule
    {
        public bool isAlive { get; set; } = false;
        public int x { get; set; }
        public int y { get; set; }
        public bool? isInLifeNext { get; set; }
        public bool MustBeDisplay { get; set; } 
    }
}
