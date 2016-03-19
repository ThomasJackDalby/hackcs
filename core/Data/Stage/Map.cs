using HackCS.Core.Data.Stage;
using HackCS.Core.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core
{
    public class Map
    {
        public List<List<Tile>> Tiles { get; set; }
        public int MapWidth { get { return Tiles[0].Count; } }
        public int MapLength { get { return Tiles.Count; } }

        public Map(string file)
        {
            //  Tiles = ParseMapFile(file);
        }

        public Map()
        {
            Tiles = new List<List<Tile>>();
        }
        public void CreateBasicMap(int width, int length)
        {
            Random rand = new Random();
            Console.WriteLine("Creating map {0} wide by {1} long", width, length);
            Tiles = new List<List<Tile>>();
            for (int y = 0; y < length; y++)
            {
                List<Tile> list = new List<Tile>();
                for (int x = 0; x < width; x++)
                {
                    Tile tile = new Tile(x, y);
                    if (rand.NextDouble() > 0.7) tile.IsSolid = true;
                    list.Add(tile);
                }
                Tiles.Add(list);
            }
        }
        public char[][] ConvertToCharArray()
        {
            char[][] map = new char[Tiles.Count][];
            for (int j = 0; j < Tiles.Count; j++)
            {
                char[] charList = new char[Tiles[j].Count];
                for (int i = 0; i < Tiles[j].Count; i++) charList[i] = Tiles[j][i].Symbol;
                map[j] = charList;
            }
            return map;
        }
        public bool CheckLimits(double x, double y)
        {
            if (x < 0 || x >= MapWidth) return false;
            if (y < 0 || y >= MapLength) return false;
            return true;
        }
        public Tile TryGetTile(int x, int y)
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
        public Tile GetRandomTile()
        {
            while (true)
            {
                int x = Game.Random.Next(MapWidth);
                int y = Game.Random.Next(MapLength);
                Tile tile = TryGetTile(x, y);
                if (tile == null) continue;
                if (tile.IsSolid) continue;
                return tile;
            }
        }

        public static Map CreateFromArray(string[] data)
        {
            Map map = new Map();
            for (int y = 0; y < data.Length; y++)
            {
                string line = data[y];
                List<Tile> list = new List<Tile>();
                for (int x = 0; x < line.Length; x++)
                {
                    char symbol = line[x];
                    Tile tile = new Tile(x, y);
                    tile.Symbol = symbol;
                    if (symbol != ' ') tile.IsSolid = true;

                    list.Add(tile);
                }
                map.Tiles.Add(list);
            }
            return map;
        }
    }
}
