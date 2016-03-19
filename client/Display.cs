using HackCS.Core;
using HackCS.Core.Infos;
using HackCS.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Client
{
    public class Display : ConsoleDisplay
    {
        #region Properties
        public List<string> Messages { get; set; }

        public ConsolePanel MainPanel { get; set; }
        public ConsolePanel HUDPanel { get; set; }
        public ConsolePanel MenuPanel { get; set; }
        public ConsolePanel MessagePanel { get; set; }
        #endregion

        public Display()
        {
            Messages = new List<string>();
            //HUDPanel = new ConsolePanel(20, 1) { Left = 0, Top = 0 };
            MainPanel = new ConsolePanel(20, 20) { Left = 0, Top = 0 }; //HUDPanel.Top + HUDPanel.Height };
            //MessagePanel = new ConsolePanel(30, 8) { Left = 0, Top = MainPanel.Top + MainPanel.Height };

            SetPanels();
        }

        #region Methods
        public void SetPanels()
        {
            Panels.Clear();
            Panels.Add(MainPanel);
            //Panels.Add(HUDPanel);
            //Panels.Add(MessagePanel);
        }
        public void Draw(MapInfo map, GameInfo info)
        {
            //DrawHUD(info);
            DrawMap(map, info);
            //DrawMessages();
            WritePanels();
        }
        public void DrawHUD(GameInfo game)
        {
            foreach(PlayerInfo player in game.Players) HUDPanel.Write(String.Format("{0}:{1} ", player.Name, player.Frags), player.Colour); //String.Format("Time {0:hh:mm:ss}\n", game.GameTime), ConsoleColor.Gray);
        }
        public void DrawMap(MapInfo map, GameInfo info)
        {
            int viewWidth = 18;
            int viewHeight = 18;
            int viewX = info.ExtendedPlayerInfo.X - viewWidth / 2;
            int viewY = info.ExtendedPlayerInfo.Y - viewHeight / 2;
            if (viewX < 0) viewX = 0;
            if (viewY < 0) viewY = 0;
            if (viewX + viewWidth > map.MapWidth) viewX = map.MapWidth - viewWidth;
            if (viewY + viewHeight > map.MapLength) viewY = map.MapLength - viewHeight;

            for (int j = 0; j < viewHeight; j++)
            {
                char[] line = new char[viewWidth];
                for (int i = 0; i < line.Length; i++)
                {
                    TileInfo tile = map.Tiles[viewY + j][viewX + i];
                    if (tile.Player != null)
                    {
                        if (tile.Player.IsKilled)
                        {
                            char symbol = 'X';
                            ConsoleColor colour = ConsoleColor.Red;  //tile.Player.Colour;
                            MainPanel.Write(symbol, colour);
                        }
                        else
                        {
                            char symbol = tile.Player.Symbol;
                            ConsoleColor colour = tile.Player.Colour;  //tile.Player.Colour;
                            MainPanel.Write(symbol, colour);
                        }
                    }
                    else if (tile.Bullet != null)
                    {
                        char symbol = '.';
                        ConsoleColor colour = ConsoleColor.Yellow;
                        MainPanel.Write(symbol, colour);
                    }
                    else
                    {
                        char symbol = tile.Symbol;
                        ConsoleColor colour = tile.Colour;
                        MainPanel.Write(symbol, colour);
                    }
                }
                MainPanel.Write('\n', ConsoleColor.Gray);
            }
            int p = 1;
        }
        public void DrawMessages()
        {
            for (int i = 0; i < Messages.Count; i++) MessagePanel.Write(String.Format("{0}\n", Messages[i]), ConsoleColor.Gray);
        }
        public void Info(string message)
        {
            Messages.Insert(0, message);
            if (Messages.Count > 5) Messages.RemoveRange(5, Messages.Count - 5);
        }
        #endregion
    }
}
