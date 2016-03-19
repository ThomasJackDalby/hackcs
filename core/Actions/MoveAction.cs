using HackCS.Core;
using HackCS.Core.Actions;
using HackCS.Core.Data.Actors;
using HackCS.Core.Data.Stage;
using HackCS.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core
{
    public class MoveAction : BaseAction
    {
        public Direction Direction { get; set; }
        public override void Process(Player player, Game game)
        {
            int newX = RTSTools.GetX(player.X, Direction);
            int newY = RTSTools.GetY(player.Y, Direction);

            // is space off map
            if (!game.Map.CheckLimits(newX, newY))
            {
                return;
            }

            // Is space collision free
            Tile tile = game.Map.Tiles[newY][newX];
            if (tile.IsSolid)
            {
                return;
            }
            if (tile.Player != null)
            {
                if (tile.Player is Bullet) //TODO put something here

                return;
            }

            Move(player, tile);
        }
        public void Move(Player actor, Tile tile)
        {
            if (Constants.Debug) Console.WriteLine("{0} moved {1}", actor.Name, Direction);
            actor.Tile.Player = null;

            actor.X = tile.X;
            actor.Y = tile.Y;

            tile.Player = actor;
            actor.Tile = tile;
        }
    }
}
