using HackCS.Core.Data.Actors;
using HackCS.Core.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HackCS.Core
{
    public class Tile
    {
        public bool IsSolid { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Player Player { get; set; }
        public Bullet Bullet { get; set; }
        public char Symbol { get; set; }
        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
