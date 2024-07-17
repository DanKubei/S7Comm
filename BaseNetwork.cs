using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace Network
{
    class BaseNetwork : NetworkInterface
    {
        private Thread _clientThread, _serverThread;
        private Socket _tcpClient, _tcpServer;
        private string _plcIp;
        private Queue<byte[]> _packets = new Queue<byte[]>();

        public BaseNetwork(string ownIp, string plcIp)
        {
            _plcIp = plcIp;

            _tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpClient.Bind(new IPEndPoint(IPAddress.Parse(ownIp), 0));

            _tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpServer.Bind(new IPEndPoint(IPAddress.Parse(ownIp), 55557));
            _tcpServer.Listen();

            _serverThread = new Thread(Recieve);
            _clientThread = new Thread(Send);

            _serverThread.Start();
            _clientThread.Start();
        }

        private async void Recieve()
        {
            while (true)
            {
                using Socket tcpSession = await _tcpServer.AcceptAsync();
                while (true)
                {
                    List<byte> response = [];
                    byte[] buffer = new byte[512];
                    int bytes = 0;
                    do
                    {
                        bytes = await tcpSession.ReceiveAsync(buffer);
                        response.AddRange(buffer.Take(bytes));
                    }
                    while (bytes > 0);
                    InvokeOnPacketReceived(response.ToArray());
                }
            }
        }

        private async void Send()
        {
            while (true)
            {
                try
                {
                    await _tcpClient.ConnectAsync(_plcIp, 5757);
                    if (_packets != null)
                    {
                        await _tcpClient.SendAsync(_packets.Dequeue());
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex) { }
            }
        }

        public override void SendPacket(byte[] packet)
        {
            _packets.Enqueue(packet);
        }
    }
}
