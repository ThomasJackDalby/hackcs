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
using HackCS.Core.Data.Actors;
using HackCS.Core.Data.Stage;

namespace HackCS.Server.Server
{
    public class Program
    {
        public ServerManananger Server { get; set; }
        public bool InGame { get; set; }
        public string HostIP { get; set; }

        Map LoadedMap { get; set; }

        public Game Game { get; set; }
        public ThreadSafeList<Player> RemovePlayers { get; set; }
        public ThreadSafeList<Player> AddPlayers { get; set; }
        public ThreadSafeList<string> Messages { get; set; }

        public Program()
        {
            HostIP = "10.13.110.201";
            LoadMap("default.txt");

            Console.WriteLine("Welcome to Hack CS Server");
            while (true)
            {
                List<MenuOption> mainMenu = new List<MenuOption>();
                mainMenu.Add(new MenuOption("setmap", (o) => LoadMap(o[0])) { Description = "Load a map" });
                mainMenu.Add(new MenuOption("setip", (o) => HostIP = o[0]) { Description = "Set IP address of Hack CS server" });
                mainMenu.Add(new MenuOption("host", (o) => { Host(); }) { Description = "Host a game" });
                ConsoleMenu.ShowMenu(mainMenu);
            }
        }

        public void LoadMap(string filename)
        {
            string[] data = File.ReadAllLines(@"Maps\" + filename);
            LoadedMap = Map.CreateFromArray(data);
        }

        public void Host()
        {
            Setup();
            ThreadPool.QueueUserWorkItem(o => RunGame());
            ThreadPool.QueueUserWorkItem(o => ShowServerMenu());
            while (InGame) { }
        }

        public void Setup()
        {
            AddPlayers = new ThreadSafeList<Player>();
            RemovePlayers = new ThreadSafeList<Player>();
            Messages = new ThreadSafeList<string>();

            Game = new Game();
            if (LoadedMap != null) Game.Map = LoadedMap;        
            else
            {
                Game.Map = new Map();
                Game.Map.CreateBasicMap(30, 30);
            }

            // Setup connections
            Server = new ServerManananger();
            Server.NewUserMethod = AddNewUser;
            Server.RemoveUserMethod = RemoveUser;
            Server.UserInputMethod = ReceiveUserInputs;
            ThreadPool.QueueUserWorkItem(o => Server.Start(HostIP));
            InGame = true;
        }

        public void ShowServerMenu()  
        {
            Console.WriteLine("------------ Server Menu -----------");
            while (InGame)
            {
                List<MenuOption> mainMenu = new List<MenuOption>();
                mainMenu.Add(new MenuOption("info", (o) => Info()));
                mainMenu.Add(new MenuOption("players", (o) => ShowPlayers()));
                mainMenu.Add(new MenuOption("close", (o) => Close()));
                ConsoleMenu.ShowMenu(mainMenu);
            }
        }

        public void Info()
        {

        }
        public void ShowPlayers()
        {
            Game.Players.ForEach(p => Console.WriteLine("{0}", p.Name));
        }
        public void Close()
        {

        }


        public void RunGame()
        {         
            while (InGame)
            {
                RemovePlayersSafe();
                AddPlayersSafe();
                Game.RunGameLoopOnce();
                BroadcastGameState();



                Thread.Sleep(10);
            }
            Console.WriteLine("Ended the game");
        }

        public void RemovePlayersSafe()
        {
            object _lock = new object();
            lock (_lock)
            {
                RemovePlayers.ForEach(o => Game.Players.Remove(o));
                RemovePlayers.Clear();
            }
        }
        public void AddPlayersSafe()
        {
            object _lock = new object();
            lock (_lock)
            {
                AddPlayers.ForEach(o => Game.AddPlayer(o));
                AddPlayers.Clear();
            }
        }



        public void AddNewUser(Player player, string jsonPlayerInfo)
        {
            try
            {
                PlayerInfo playerInfo = (PlayerInfo)JsonConvert.DeserializeObject(jsonPlayerInfo, typeof(PlayerInfo));
                player.Name = playerInfo.Name;
                player.Colour = playerInfo.Colour;
                player.Symbol = playerInfo.Symbol;

                string jsonMap = JsonConvert.SerializeObject(Game.Map.ConvertToCharArray());
                Server.SendMessage(player, jsonMap);

                AddPlayers.Add(player);
                Console.WriteLine(String.Format("Player {0} joined the game.", player.Name));
            }
            catch(Exception e)
            {
                Console.WriteLine("{0} completely failed to enter there data correctly. Well done muppet");
            }
        }
        public void RemoveUser(Player player)
        {
            RemovePlayers.Add(player);
            Messages.Add(String.Format("{0} left the game, like the coward they are!", player.Name));
            Console.WriteLine(String.Format("Player {0} left the game.", player.Name));
        }
        public void ReceiveUserInputs(Player player, string command)
        {
            ActionInfo actionInfo = (ActionInfo)JsonConvert.DeserializeObject(command, typeof(ActionInfo));

            switch(actionInfo.Action)
            {
                case "move": player.ActionType = new MoveAction { Direction = (Direction)Enum.Parse(typeof(Direction), actionInfo.Args[0]) };
                    return;
                case "shoot": player.ActionType = new ShootAction { Direction = (Direction)Enum.Parse(typeof(Direction), actionInfo.Args[0]) };
                    return;
                default:
                    return;
            }
        }
        public void BroadcastGameState()
        {
            List<PlayerInfo> playerInfos = new List<PlayerInfo>();
            Game.Players.ForEach(player => playerInfos.Add(player.GetInfo()));

            List<BulletInfo> bulletInfos = new List<BulletInfo>();
            Game.BulletList.ForEach(player => bulletInfos.Add(player.GetInfo()));

            Game.Players.ForEach(player =>{
                try
                {
                    GameInfo gameInfo = new GameInfo();
                    gameInfo.Players = playerInfos;
                    gameInfo.ExtendedPlayerInfo = new ExtendedPlayerInfo() { X = player.X, Y = player.Y };
                    gameInfo.Bullets = bulletInfos;

                    Game.Messages.ForEach(m => gameInfo.Messages.Add(m));
                    Messages.ForEach(m => gameInfo.Messages.Add(m));

                    string jsonGameInfo = JsonConvert.SerializeObject(gameInfo);
                    Server.SendMessage(player, jsonGameInfo);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Bug");
                }
            });

            Game.Messages.Clear();
            Messages.Clear();
        }
        static void Main(string[] args)
        {
            new Program();
        }
    }
}