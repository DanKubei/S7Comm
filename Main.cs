using Network;
using Packers;
using Protocols.S7;
using Protocols.S7.Parameters;
using Protocols.COTP.Parameters;
using Model;
using Console = System.Console;
using Protocols.COTP;
using Protocols;


namespace ConsoleView
{

    class Programm
    {

        public static NetworkInterface networkInterface;
        public static CotpPacker cotpPacker;
        public static S7CommCommunicationManager s7CommManager;
        public static Thread s7LoopThread;

        public static void Main() 
        {
            networkInterface = new BaseNetwork("192.168.0.103", "192.168.0.105");
            cotpPacker = new CotpPacker(3);
            s7CommManager = new S7CommCommunicationManager(networkInterface);

            //S7CommCommunicationManager client = new S7CommCommunicationManager(networkInterface);
            //Cotp cotp = new CotpConnectionRequest([0, 0], [0xb4, 0x50], 0, [
            //    new CotpParameterTpduSize(512),
            //    new CotpParameterSourceTsap([0x20,0]),
            //    new CotpParameterDestinationTsap([0x21,0])
            //    ]);
            s7LoopThread = new Thread(S7Loop);
            SetupConnection();
            Console.ReadLine();
        }

        public static void SetupConnection()
        {
            Cotp cotp = new CotpConnectionRequest([0, 0], [0xb4,0x50], 0, [
                new CotpParameterTpduSize(512),
                new CotpParameterSourceTsap([0x20, 0]),
                new CotpParameterDestinationTsap([0x21, 0])
                ]);
            networkInterface.SendPacket(cotpPacker.Pack(cotp));
            networkInterface.OnPacketReceived += OnCotpCrResponse;
        }

        public static void OnCotpCrResponse(byte[] packet)
        {
            byte[] buffer;
            TPKT? tpkt = TPKT.ParsePacket(packet, out buffer);
            if (tpkt == null)
            {
                return;
            }
            packet = buffer;
            Cotp? cotp = Cotp.ParsePacket(packet, out buffer);
            if (cotp == null)
            {
                return;
            }
            if (packet.GetType() == typeof(CotpConnectionRequest))
            {
                SendS7SetupCommunication();
                networkInterface.OnPacketReceived -= OnCotpCrResponse;
            }
        }

        public static void SendS7SetupCommunication()
        {
            s7CommManager.SendPacket(
                S7Comm.MessageType.Job,
                [ new S7CommParameterSetupCommunication([0,4], [0,4], [0,240])],
                []);
            s7CommManager.OnPacketReceived += OnS7SetupCommunication;
        }

        public static void OnS7SetupCommunication(S7Comm packet)
        {
            if (packet.messageType != S7Comm.MessageType.AckData)
            {
                return;
            }
            if (packet.parameters[0].GetType() == typeof(S7CommParameterSetupCommunication))
            {
                s7CommManager.OnPacketReceived -= OnS7SetupCommunication;
                s7LoopThread.Start();
            }
        }

        public static void S7Loop()
        {
            s7CommManager.OnPacketReceived += S7Recieve;
            while (true) 
            {
                s7CommManager.SendPacket(
                    S7Comm.MessageType.Job,
                    [new S7CommParameterWriteVar(12, 0x10, 1, [0, 1], [0, 1], 0x81, [0,0,1])],
                    [new S7CommData(S7CommData.ResponseCode.Success, S7CommData.TransportSize.Bit, [1])]
                    );
                s7CommManager.SendPacket(
                    S7Comm.MessageType.Job,
                    [new S7CommParameterWriteVar(12, 0x10, 1, [0, 1], [0, 1], 0x81, [0, 0, 2])],
                    [new S7CommData(S7CommData.ResponseCode.Success, S7CommData.TransportSize.Bit, [0])]
                    );
                Thread.Sleep(1000);
                s7CommManager.SendPacket(
                    S7Comm.MessageType.Job,
                    [new S7CommParameterWriteVar(12, 0x10, 1, [0, 1], [0, 1], 0x81, [0, 0, 1])],
                    [new S7CommData(S7CommData.ResponseCode.Success, S7CommData.TransportSize.Bit, [0])]
                    );
                s7CommManager.SendPacket(
                    S7Comm.MessageType.Job,
                    [new S7CommParameterWriteVar(12, 0x10, 1, [0, 1], [0, 1], 0x81, [0, 0, 2])],
                    [new S7CommData(S7CommData.ResponseCode.Success, S7CommData.TransportSize.Bit, [1])]
                    );
            }
        }

        public static void S7Recieve(S7Comm packet)
        {
        
        }

    }

}