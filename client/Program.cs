using Newtonsoft.Json;
using HackCS.Core;
using HackCS.Core.Actions;
using HackCS.Core.Infos;
using HackCS.Core.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HackCS.Client
{
    public class Program
    {
        public static Random Random = new Random();
        public TCPClient Client { get; set; }
        public MapInfo Map { get; set; }
        public Display Display { get; set; }

        public string HostIP { get; set; }
        public PlayerInfo Player { get; set; }

        public bool InGame { get; set; }

        public Program()
        {
            Display = new Display() { Height = 20, Width = 20 };
            Player = new PlayerInfo();
            InGame = false;
            Console.WriteLine("Welcome to Hack CS");

            HostIP = "10.13.110.201";
            string[] randomNames = new string[] { "Pyro", "Heavy", "Spy", "Scout", "Soldier" };
            Player.Name = randomNames[Random.Next(randomNames.Length-1)];
            Player.Symbol = 'O';
            Player.Colour = (ConsoleColor)Enum.GetValues(typeof(ConsoleColor)).GetValue(Random.Next(Enum.GetValues(typeof(ConsoleColor)).Length));

            ThreadPool.QueueUserWorkItem(o => ShowMainMenu());

            while (true)
            { } // Never quit
        }

        public void ShowMainMenu()
        {
            Console.WriteLine("------------ Main Menu -----------");
            while (!InGame)
            {
                List<MenuOption> mainMenu = new List<MenuOption>();
                mainMenu.Add(new MenuOption("setname", (o) => Player.Name = o[0]) { Description = "Set player name" });
                mainMenu.Add(new MenuOption("load", (o) => Load()) { Description = "Load" });
                mainMenu.Add(new MenuOption("save", (o) => Save()) { Description = "Save" });
                mainMenu.Add(new MenuOption("setchar", (o) => Player.Symbol = o[0][0]) { Description = "Set players symbol" });
                mainMenu.Add(new MenuOption("setcolour", (o) => Player.Colour = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), o[0])) { Description = "Set player colour" });
                mainMenu.Add(new MenuOption("setip", (o) => HostIP = o[0]) { Description = "Set IP address of Hack CS server" });
                mainMenu.Add(new MenuOption("info", (o) =>
                {
                    Console.WriteLine("----------");
                    Console.WriteLine("Name: " + Player.Name);
                    Console.WriteLine("Colour: " + Player.Colour);
                    Console.WriteLine("Symbol: " + Player.Symbol);
                    Console.WriteLine("IP: " + HostIP);
                }) { Description = "Display player information" });
                mainMenu.Add(new MenuOption("connect", (o) => { StartGame(); }) { Description = "Connect to a game" });
                ConsoleMenu.ShowMenu(mainMenu);
            }
        }


        public void Save()
        {
            string filename = "options.txt";
            string jsonSave = JsonConvert.SerializeObject(Player);
            File.WriteAllText(filename, jsonSave);
        }

        public void Load()
        {
            string filename = "options.txt";
            string jsonText = File.ReadAllText(filename);
            Player = (PlayerInfo)JsonConvert.DeserializeObject<PlayerInfo>(jsonText);
        }

        public void StartGame()
        {
            try
            {
                InGame = true;
                Connect();
                ThreadPool.QueueUserWorkItem(o => SendUserInput());
                ThreadPool.QueueUserWorkItem(o => UpdateGame());
            }
            catch (Exception e)
            {                        
                Console.WriteLine("Unable to connect to that game/ip");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                InGame = false;
                ThreadPool.QueueUserWorkItem(o => ShowMainMenu());
            }
        }

        public void Connect()
        {
            bool fail = true;
            while(fail)
            {
                try
                {
                    Client = new TCPClient();
                    string jsonPlayer = JsonConvert.SerializeObject(Player);
                    string mapData = Client.Connect(HostIP, jsonPlayer);
                    char[][] mapCharArray = JsonConvert.DeserializeObject<char[][]>(mapData);
                    Map = MapInfo.LoadFromArray(mapCharArray);
                    fail = false;
                }
                catch (Exception e) 
                {
                    Thread.Sleep(100);                
                }
            }

        }

        public void SendUserInput()
        {
            while (InGame)
            {
                if (Console.KeyAvailable)
                {
                    ActionInfo action = null;

                    ConsoleKeyInfo key = Console.ReadKey(false);
                    if (key.Modifiers == default(ConsoleModifiers))
                    {
                        if (key.Key == ConsoleKey.LeftArrow) action = Move(Direction.Left);
                        else if (key.Key == ConsoleKey.RightArrow) action = Move(Direction.Right);
                        else if (key.Key == ConsoleKey.UpArrow) action = Move(Direction.Up);
                        else if (key.Key == ConsoleKey.DownArrow) action = Move(Direction.Down);
                        else if (key.Key == ConsoleKey.A) action = Shoot(Direction.Left);
                        else if (key.Key == ConsoleKey.D) action = Shoot(Direction.Right);
                        else if (key.Key == ConsoleKey.W) action = Shoot(Direction.Up);
                        else if (key.Key == ConsoleKey.S) action = Shoot(Direction.Down);
                    }

                    if (action == null) continue;

                    Thread.Sleep(75);
                    string jsonAction = JsonConvert.SerializeObject(action);
                    Client.SendMessage(jsonAction);
                }
            }
        }

        public ActionInfo Move(Direction direction)
        {
            ActionInfo action = new ActionInfo() { Action = "move", Args = new string[] { direction.ToString() } };
            return action;
        }
        public ActionInfo Shoot(Direction direction)
        {
            ActionInfo action = new ActionInfo() { Action = "shoot", Args = new string[] { direction.ToString() } };
           // ThreadPool.QueueUserWorkItem(o => Console.Beep(5000, 200));
            return action;
        }

        public void UpdateGame()
        {
            int errorCount = 0;
            int errorLimit = 10;
            int delay = 75;
            while (InGame)
            {
                try
                {
                    string jsonWorldInfo = Client.ReadMessage();
                    GameInfo gameInfo = (GameInfo)JsonConvert.DeserializeObject(jsonWorldInfo, typeof(GameInfo));

                    Map.Clear();
                    Map.Update(gameInfo);

                    Console.Clear();
                    foreach (string message in gameInfo.Messages) Display.Info(message);
                    Display.Draw(Map, gameInfo);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Something went wrong {0}/{1}", errorCount, errorLimit);
                    Console.WriteLine(e.Message);
                    errorCount++;
                    if (errorCount > errorLimit)
                    {
                        InGame = false;
                        ThreadPool.QueueUserWorkItem(o => ShowMainMenu());
                    }
                }
                Thread.Sleep(delay);
            }
        }

        static void Main(string[] args)
        {
            new Program();
        }
    }
}
