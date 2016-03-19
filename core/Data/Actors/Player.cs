using HackCS.Core;
using HackCS.Core.Actions;
using HackCS.Core.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Data.Actors
{
    public class Player
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsKilled { get; set; }

        public int Kills { get; set; }
        public int Deaths { get; set; }

        public ConsoleColor Colour { get; set; }
        public Tile Tile { get; set; }
        public char Symbol { get; set; }
        public BaseAction ActionType { get; set; }
        public int DeathTimer { get; set; }
        public Player()
        {
            IsKilled = false;
        }

        public PlayerInfo GetInfo()
        {
            return new PlayerInfo { X = X, Y = Y, Name = Name, Symbol = Symbol, Colour = Colour, IsKilled = IsKilled, Frags = Kills };
        }
    }
}
