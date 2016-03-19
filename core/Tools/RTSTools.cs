using HackCS.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Tools
{
    public static class RTSTools
    {
        public static List<string> ReadFile(string filename)
        {
            List<string> data = null;
            try { data = File.ReadAllLines(filename).ToList(); }
            catch (Exception)
            {
                // TODO Implement warning/logging here.
                Console.WriteLine("Unable to read file");
            }
            return data;
        }
        public static List<List<string>> ReadDelimitedFile(string filename, char delimiter)
        {
            List<string> data = new List<string>();
            List<List<string>> output = new List<List<string>>();
            data = ReadFile(filename);
            foreach (string line in data)
            {
                List<string> lineData = new List<string>();
                lineData = Split(line, delimiter);
                output.Add(lineData);
            }
            return output;
        }
        public static List<string> Split(string line, char delimiter)
        {
            List<string> data = new List<string>();
            data = line.Split(delimiter).ToList();
            return data;
        }
        public static double GetDistance(Tile tileA, Tile tileB)
        {
            return GetDistance(tileA.X, tileA.Y, tileB.X, tileB.Y);
        }
        public static double GetDistance(int x1, int y1, int x2, int y2)
        {
            double delx = x2 - x1;
            double dely = y2 - y1;
            return Math.Sqrt(delx * delx + dely * dely);
        }
        public static int GetX(int x, Direction direction)
        {
            if (direction == Direction.Left) return x - 1;
            if (direction == Direction.Right) return x + 1;
            return x;
        }
        public static int GetY(int y, Direction direction)
        {
            if (direction == Direction.Up) return y - 1;
            if (direction == Direction.Down) return y + 1;
            return y;
        }
    }
}
