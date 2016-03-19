using HackCS.Core.Data.Actors;
using HackCS.Core.Data.Stage;
using HackCS.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Actions
{
    public class ShootAction : BaseAction
    {
        public Direction Direction { get; set; }
        public override void Process(Player player, Game game)
        {
            Bullet bullet = new Bullet();
            bullet.Shooter = player;
            bullet.Direction = Direction;
            bullet.X = RTSTools.GetX(player.X, Direction);
            bullet.Y = RTSTools.GetY(player.Y, Direction);

            Tile tile = game.Map.TryGetTile(bullet.X, bullet.Y);
            if (tile == null) return;
            if (tile.IsSolid) return;

            //TODO might need to handle multple bullets being in the same place

            if (Constants.Debug) Console.WriteLine("{0} fired {1}", player.Name, Direction);
            bullet.Tile = tile;
            tile.Bullet = bullet;
            game.BulletList.Add(bullet);
        }
    }
}
