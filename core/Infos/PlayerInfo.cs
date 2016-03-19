using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Infos
{
    public class PlayerInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public int Frags { get; set; }

        public bool IsKilled { get; set; }
        public ConsoleColor Colour { get; set; }
        public char Symbol { get; set; }
    }
}
