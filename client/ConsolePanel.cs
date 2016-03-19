using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Client
{
    public class ConsolePanel
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public char[] Buffer { get; set; }
        public char[][] Data { get; set; }
        public ConsoleColor[] ColourBuffer { get; set; }
        public ConsoleColor[][] ColourData { get; set; }
        public int BufferIndex { get; set; }
        public int BufferSize { get; set; }

        public ConsolePanel(int width, int height)
        {
            Width = width;
            Height = height;
            BufferSize = (width+1) * (height+1);
            Buffer = new char[BufferSize];
            ColourBuffer = new ConsoleColor[BufferSize];
            for (int i = 0; i > ColourBuffer.Length; i++) ColourBuffer[i] = ConsoleColor.Magenta;
        }

        public void Process()
        {
            string[] data = new string(Buffer).TrimEnd('\0').Split('\n');
            if (data[data.Length - 1] == "")
            {
                data = data.Take(data.Length - 1).ToArray();

            }
            char[][] charData = new char[data.Length][];

            ConsoleColor[][] colourData = new ConsoleColor[data.Length][];
            int start = 0;
            int finish = 0;
            for (int i = 0; i < data.Length;i++)
            {
                finish += data[i].Length;
                if (i!=0) finish += 1;
                colourData[i] = ColourBuffer.Skip(start).Take(finish-start).ToArray();
                start = finish + 1;
            }

            Data = new char[Height][];
            ColourData = new ConsoleColor[Height][];

            int numberOfLines = data.Length;
            if (numberOfLines > Width) numberOfLines = Height;

            for (int i = 0; i < Height; i++)
            {
                Data[i] = new char[Width];
                ColourData[i] = new ConsoleColor[Width];

                if (i >= numberOfLines)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        Data[i][j] = ' ';
                        ColourData[i][j] = ConsoleColor.Gray;
                    }
                }
                else
                {
                    int len = data[i].Length;
                    if (len > Width) len = Width;
                    for (int j = 0; j < len; j++)
                    {
                        Data[i][j] = data[i][j];
                        ColourData[i][j] = colourData[i][j];
                    }
                }
            }
        }

        public void Write(string data, ConsoleColor colour)
        {
            Write(data.ToArray(), colour);
        }
        public void Write(char data, ConsoleColor colour)
        {
            Buffer[BufferIndex] = data;
            ColourBuffer[BufferIndex] = colour;
            BufferIndex++;
        }
        public void Write(char[] data, ConsoleColor colour)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Buffer[BufferIndex] = data[i];
                ColourBuffer[BufferIndex] = colour;
                BufferIndex++;
            }
        }
    }
}
