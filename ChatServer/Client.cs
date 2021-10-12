using ChatServer.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket;

        PacketReader _packetReader;
        Thread processThread;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Username}");

            processThread = new Thread(new ThreadStart(Process));
            processThread.Start();
        }

        void Process()
        {
            bool alive = true;

            while (alive)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    System.Diagnostics.Debug.WriteLine(opcode);

                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            //Console.WriteLine($"[{DateTime.Now}]: Message received! {msg}");
                            Program.BroadCastChatMessage(msg, DateTime.Now.ToString(), Username);
                            break;
                    }
                }
                catch (Exception)
                {
                    Program.BroadCastDisconnect(UID);
                    alive = false;
                }
            }
        }
    }
}
