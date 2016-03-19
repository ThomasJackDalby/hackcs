using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Client
{
    public class ConsoleDisplay
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Dictionary<object, char> CharDictionary { get; set; }
        public Dictionary<object, ConsoleColor> ColourDictionary { get; set; }
        public ConsoleColor CurrentColour { get; set; }
        public int BufferSize { get; set; }
        public int BufferIndex { get; set; }
        public char[] Buffer { get; set; }

        public ConsoleDisplay()
        {
            Panels = new List<ConsolePanel>();
            CharDictionary = new Dictionary<object, char>();
            ColourDictionary = new Dictionary<object, ConsoleColor>();

            BufferSize = 1024;
            CurrentColour = ConsoleColor.Gray;
            Buffer = new char[BufferSize];
        }

        public char GetChar(object type)
        {
            if (!CharDictionary.Keys.Contains(type)) RegisterObject(type);
            return CharDictionary[type];
        }
        public ConsoleColor GetColour(object type)
        {
            if (!ColourDictionary.Keys.Contains(type)) RegisterObject(type);
            return ColourDictionary[type];
        }
        public void RegisterObject(object obj)
        {
            char symbol = '?';
            ConsoleColor colour = ConsoleColor.DarkGreen;
            RegisterObject(obj, symbol, colour);
        }
        public void RegisterObject(object obj, char symbol, ConsoleColor colour)
        {
            CharDictionary.Add(obj, symbol);
            ColourDictionary.Add(obj, colour);
        }
        public void ChangeColour(ConsoleColor colour)
        {
            if (colour != CurrentColour)
            {
                Flush();
                Console.ForegroundColor = colour;
                CurrentColour = colour; 
            }
        }
        public void Write(string data)
        {
            Write(data.ToArray());
        }
        public void Write(char data)
        {
            Buffer[BufferIndex++] = data;
        }
        public void Write(char[] data)
        {
            for (int i = 0; i < data.Length; i++) Buffer[BufferIndex++] = data[i];
        }
        public void Flush()
        {
            char[] temp = new char[BufferIndex];
            for (int i = 0; i < BufferIndex; i++) temp[i] = Buffer[i];
            Console.Write(temp);
            Buffer = new char[BufferSize];
            BufferIndex = 0;
        }
        public void Clear()
        {
            Console.Clear();
        }

        public List<ConsolePanel> Panels { get; set; }
        public void WritePanels()
        {
            foreach (var panel in Panels) panel.Process();
            Panels.Sort((a, b) =>
            {
                if (a.Left < b.Left) return 1;
                else if (a.Left > b.Left) return -1;
                else return 0;
            });

            for (int y = 0; y < Height; y++)
            {

                int x = 0;
                while(x<Width)
                {
                    ConsolePanel panel = Panels.SingleOrDefault(p => (p.Top <= y && p.Top + p.Height > y && p.Left == x));
                    if (panel == null)
                    {
                        ChangeColour(ConsoleColor.DarkGray);
                        Write('#');
                        x++;
                        continue;
                    }

                    for(int i=0;i<panel.Width;i++)
                    {
                        ChangeColour(panel.ColourData[y - panel.Top][x - panel.Left]);
                        Write(panel.Data[y - panel.Top][x - panel.Left]);
                        x++;
                    }
                }
                Write('\n');
            }
            Flush();

            foreach(var panel in Panels)
            {
                panel.Buffer = new char[BufferSize];
                panel.ColourBuffer = new ConsoleColor[BufferSize];

                for (int i = 0; i > panel.ColourBuffer.Length; i++) panel.ColourBuffer[i] = ConsoleColor.Magenta;
                panel.BufferIndex = 0;
            }
        }
    }
}
