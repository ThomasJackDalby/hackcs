using HackCS.Core;
using HackCS.Core.Data.Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HackCS.Server.Server
{
    public class ServerManananger
    {
        public PlayerAction<string> NewUserMethod { get; set; }
        public Action<Player> RemoveUserMethod { get; set; }
        public PlayerAction<string> UserInputMethod { get; set; }
        public Dictionary<Player, ClientHandler> PlayerClientDictionary { get; set; }

        public ServerManananger()
        {
            PlayerClientDictionary = new Dictionary<Player, ClientHandler>();
        }

        public void Start(string ipString)
        {
            IPAddress ip = IPAddress.Parse(ipString);
            TcpListener serverSocket = new TcpListener(ip, 8888);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();

            counter = 0;
            while ((true))
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                clientSocket.SendBufferSize = Constants.ServerToClientBufferSize;
                clientSocket.ReceiveBufferSize = Constants.ClientToServerBufferSize;

                byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);

                if (!dataFromClient.Contains('$'))
                {
                    Console.WriteLine("Received bad data from new joiner");
                    Console.WriteLine(dataFromClient);
                    continue;
                }
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                Player player = new Player();
                ClientHandler client = new ClientHandler();
                PlayerClientDictionary.Add(player, client);
                client.UserInputMethod = UserInputMethod;
                client.Start(player, clientSocket);

                NewUserMethod(player, dataFromClient);
            }
        }
        public void SendMessage(Player player, string data)
        {
            SendMessage(PlayerClientDictionary[player], data);
        }
        public void SendMessage(ClientHandler client, string data)
        {
            try
            {
                NetworkStream broadcastStream = client.clientSocket.GetStream();
                Byte[] broadcastBytes = Encoding.ASCII.GetBytes(data + "$");
                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
            catch(Exception e)
            {
                Console.WriteLine("Was unable to send a message to {0}", client.Player.Name);
                CloseClient(client);
            }
        }

        public void CloseClient(ClientHandler client)
        {
            try
            {
                Console.WriteLine("Closing connection to {0}", client.Player.Name);
                Player player = PlayerClientDictionary.Keys.Single(x => PlayerClientDictionary[x] == client);
                RemoveUserMethod(player);
                client.IsRunning = false;
                client.clientSocket.Close();      
                PlayerClientDictionary.Remove(player);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: Couldn't remove {0}", client.Player.Name);
            }
        }
    }

    public delegate void PlayerAction<T>(Player player, T data);

    public class ClientHandler
    {
        public Player Player { get; set; }
        public TcpClient clientSocket { get; set; }
        public bool IsRunning { get; set; }
        public string clNo { get; set; }
        public PlayerAction<string> UserInputMethod { get; set; }

        public void Start(Player player, TcpClient inClientSocket)
        {
            IsRunning = true;
            Player = player;
            clientSocket = inClientSocket;
            Thread ctThread = new Thread(Listen);
            ctThread.Start();
        }
        private void Listen()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while (IsRunning)
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);

                    if (!dataFromClient.Contains('$'))
                    {
                        Console.WriteLine("Received bad data from {0}", Player.Name);
                        continue;
                    }
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    UserInputMethod(Player, dataFromClient);
                    rCount = Convert.ToString(requestCount);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Was unable listen a message from {0}", Player.Name);
                }
            }
        }
    }
}