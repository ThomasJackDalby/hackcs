using HackCS.Core.Data.Actors;
using HackCS.Core.Data.Stage;
using HackCS.Core.Infos;
using HackCS.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Data.Actors
{
    public class Bullet : Actor
    {
        public Player Shooter { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Direction Direction { get; set; }
        public Tile Tile { get; set; }
        public bool Collided { get; set; }

        private int Timer { get; set; }

        public Bullet()
        {
            Timer = 0;

            Collided = false;
        }
        public BulletInfo GetInfo()
        {
            return new BulletInfo { X = X, Y = Y };
        }
        public override void Process(Game game)
        {
            if (Timer++ < Constants.BulletSpeedLimit) return;
            Timer = 0;

             int newX = RTSTools.GetX(X, Direction);
            int newY = RTSTools.GetY(Y, Direction);

            // is space off map
            if (!game.Map.CheckLimits(newX, newY))
            {
                Collided = true;
                return;
            }

            // Is space collision free
            Tile tile = game.Map.Tiles[newY][newX];
            if (tile.IsSolid) Collided = true;

            if (tile.Player != null)
            {
                Shooter.Kills++;
                game.Messages.Add(String.Format("{0} fragged {1}", Shooter.Name, tile.Player.Name));
                tile.Player.IsKilled = true;
            }

            Move(this, tile);
        }
        public void Move(Bullet bullet, Tile tile)
        {
            bullet.Tile.Bullet = null;

            bullet.X = tile.X;
            bullet.Y = tile.Y;

            tile.Bullet = bullet;
            bullet.Tile = tile;
        }
    }
}
