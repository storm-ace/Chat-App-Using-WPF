using RandomNameGen;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using WPFChat.MVVM.Net.IO;

namespace WPFChat.MVVM.Net
{
    class Server
    {
        TcpClient _client;

        public PacketReader packetReader;

        public event Action ConnectedEvent;
        public event Action MsgReceivedEvent;
        public event Action UserDisconnectedEvent;

        public Server()
        {
            _client = new TcpClient();
            ConnectToServer($"User: {DateTime.Now}");
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 8080);
                packetReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }

                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() => 
            {
                while (true)
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            ConnectedEvent?.Invoke();
                            break;

                        case 5:
                            MsgReceivedEvent?.Invoke();
                            break;

                        case 10:
                            UserDisconnectedEvent?.Invoke();
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
