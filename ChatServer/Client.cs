using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket;

        PacketReader _packetReader;

        private CancellationTokenSource tokenSource = null;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Username}");

            Task.Run(() => Process(/*token*/));
        }

        void Process(/*CancellationToken token*/)
        {
            var opcode = _packetReader.ReadByte();
            System.Diagnostics.Debug.WriteLine(opcode);

            while (true)
            {
                switch (opcode)
                {
                    case 5:
                        var msg = _packetReader.ReadMessage();
                        Console.WriteLine($"[{DateTime.Now}]: Message received! {msg}");
                        Program.BroadCastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                        break;
                }
            }
        }
    }
}
