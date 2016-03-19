using HackCS.Core.Data.Actors;
using HackCS.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Data.Stage
{
    public class Game
    {
        public static Random Random = new Random();

        public ThreadSafeList<Player> Players { get; set; }
        public List<Bullet> BulletList { get; set; }
        public List<Actor> ActorList { get; set; }

        public Map Map { get; set; }
        public List<string> Messages { get; set; }

        public Game()
        {
            Messages = new List<string>();
            Players = new ThreadSafeList<Player>();
            ActorList = new List<Actor>();
            BulletList = new List<Bullet>();
            Map = new Map();
        }

        public void RunGameLoopOnce()
        {
            // Process player actions
            Players.ForEach(player =>
            {
                if (player.IsKilled)
                {
                    if (player.DeathTimer++ < Constants.DeathTimeLimit) return;  
                    RespawnPlayer(player);
                }

                if (player.ActionType == null) return;
                player.ActionType.Process(player, this);
                player.ActionType = null;
            });

            // Process all Actors
            ActorList.ForEach(actor => actor.Process(this));

            // Move bullets forward by one
            BulletList.ForEach(bullet => bullet.Process(this));

            // Remove dead shots
            List<Bullet> deadBullets = BulletList.Where(x => x.Collided == true).ToList();
            deadBullets.ForEach(x => BulletList.Remove(x));
        }


        public void AddPlayer(Player player)
        {
            Players.Add(player);

            player.Tile = Map.GetRandomTile();
            player.Tile.Player = player;
            player.X = player.Tile.X;
            player.Y = player.Tile.Y;

            Messages.Add(String.Format("{0} has joined the game", player.Name));
        }

        public void RespawnPlayer(Player player)
        {
            player.Tile.Player = null;
            player.Tile = Map.GetRandomTile();
            player.Tile.Player = player;
            player.X = player.Tile.X;
            player.Y = player.Tile.Y;
            player.IsKilled = false;
            player.DeathTimer = 0;
            player.Deaths++;
        }
    }
}
