using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Infos
{
    public class MapInfo
    {
        public List<List<TileInfo>> Tiles { get; set; }
        public int MapWidth { get; set; }
        public int MapLength { get; set; }


        public MapInfo()
        {
            
        }

        public static MapInfo LoadFromArray(char[][] mapArray)
        {
            MapInfo map = new MapInfo();
            map.Tiles = new List<List<TileInfo>>();
            for (int i = 0; i < mapArray.Length; i++)
            {
                List<TileInfo> list = new List<TileInfo>();
                for (int j = 0; j < mapArray[i].Length; j++)
                {
                    list.Add(new TileInfo() { X = j, Y = i, Colour = ConsoleColor.Gray, Symbol = mapArray[i][j] });
                }
                map.Tiles.Add(list);
            }

            map.MapLength = mapArray.Length;
            map.MapWidth = mapArray[0].Length;
            return map;
        }

        public TileInfo TryGetTile(int x, int y)
        {
            try
            {
                return Tiles[y][x];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void Update(GameInfo info)
        {
            foreach(PlayerInfo player in info.Players)
            {
                TileInfo tile = TryGetTile(player.X, player.Y);
                if (tile != null)
                {
                    tile.Player = player;
                }
            }
            foreach (BulletInfo bullet in info.Bullets)
            {
                TileInfo tile = TryGetTile(bullet.X, bullet.Y);
                if (tile != null)
                {
                    tile.Bullet = bullet;
                }
            }
        }

        public void Clear()
        {
            foreach (List<TileInfo> list in Tiles)
            {
                foreach (TileInfo tile in list)
                {
                    tile.Clear();
                }
            }
        }
    }
}
