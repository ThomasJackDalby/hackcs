using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Infos
{
    public class TileInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsWall { get; set; }
        public PlayerInfo Player { get; set; }
        public BulletInfo Bullet { get; set; }
        public char Symbol { get; set; }
        public ConsoleColor Colour { get; set; }

        public TileInfo()
        {
            Colour = ConsoleColor.White;
        }

        public void Clear()
        {
            Player = null;
            Bullet = null;
        }
    }
}
