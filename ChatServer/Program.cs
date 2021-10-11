using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using WPFChat.MVVM.Net.IO;

namespace ChatServer
{
    class Program
    {
        static TcpListener _tcpListener;
        static List<Client> _users;

        static void Main(string[] args)
        {
            Console.Title = "Setting everything up...";
            _users = new List<Client>();
            _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
            _tcpListener.Start();

            while (true)
            {
                Console.Title = $"Server active | {_users.Count} online...";
                var client = new Client(_tcpListener.AcceptTcpClient());
                _users.Add(client);

                BroadCastConnection();
            }
        }

        static void BroadCastConnection()
        {
            foreach (Client user in _users)
            {
                foreach (Client usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadCastMessage(string message)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadCastDisconnect(string uid)
        {
            Client disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);

            foreach (Client user in _users)
            {
                PacketBuilder broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            BroadCastMessage($"[{disconnectedUser}] Disconnected!");
        }
    }
}
