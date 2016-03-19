using System;
using System.Text;
using System.Net.Sockets ;
using System.Threading;
using System.Net;
using HackCS.Core;

namespace HackCS.Client
{
    public class TCPClient
    {
        System.Net.Sockets.TcpClient ClientSocket { get; set; }
        NetworkStream ServerStream { get; set; }

        public TCPClient()
        {
            ClientSocket = new TcpClient();
            ClientSocket.SendBufferSize = Constants.ClientToServerBufferSize;
            ClientSocket.ReceiveBufferSize = Constants.ServerToClientBufferSize;

            ServerStream = default(NetworkStream);
        }

        public string Connect(string ipString, string jsonInfo)
        {
            IPAddress ip = IPAddress.Parse(ipString);
            ClientSocket.Connect(ip, 8888);
            ServerStream = ClientSocket.GetStream();

            // Send name initially
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(jsonInfo + "$");
            ServerStream.Write(outStream, 0, outStream.Length);
            ServerStream.Flush();

            // Receive Map
            int buffSize = ClientSocket.ReceiveBufferSize;
            Console.WriteLine("BUffer sIZE {0}", buffSize);
            byte[] inStream = new byte[buffSize];

            int i = 1;
            string jsonMap = "";
            while (!jsonMap.Contains("$"))
            {
                Console.WriteLine("Reading map " + i++);
                Console.WriteLine("Data Size: {0}", jsonMap.Length);
                ServerStream.Read(inStream, 0, buffSize);
                jsonMap += System.Text.Encoding.ASCII.GetString(inStream);
            }

            if (!jsonMap.Contains("$"))
            {
                Console.WriteLine("Received bad data from server");
                Console.WriteLine(jsonMap);
                return "";
            }
            string shortData = jsonMap.Substring(0, jsonMap.IndexOf("$"));
            return "" + shortData;

        }
        public void SendMessage(string message)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(message + "$");
            ServerStream.Write(outStream, 0, outStream.Length);
            ServerStream.Flush();
        }

        public string ReadMessage()
        {
            int buffSize = ClientSocket.ReceiveBufferSize;
            byte[] inStream = new byte[buffSize];
            ServerStream.Read(inStream, 0, buffSize);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            //TODO check the data has a $ in it
            returndata = returndata.Substring(0, returndata.IndexOf("$"));
            return "" + returndata;
        }
    }
}
