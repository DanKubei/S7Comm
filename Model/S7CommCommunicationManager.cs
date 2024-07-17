using Protocols.S7;
using Protocols.S7.Parameters;
using Network;
using Protocols;
using Protocols.COTP;
using Packers;

namespace Model
{
    class S7CommCommunicationManager
    {
        public delegate void S7CommDelegate(S7Comm s7Comm);
        public event S7CommDelegate OnPacketReceived;

        private ushort _pdur;
        private NetworkInterface _networkInterface;

        public S7CommCommunicationManager(NetworkInterface networkInterface)
        {
            _networkInterface = networkInterface;
            _networkInterface.OnPacketReceived += RecievePacket;
        }

        public void SendPacket(S7Comm.MessageType messageType, S7CommParameter[] s7CommParameters, S7CommData[] s7CommDatas, byte classEror = 0, byte error = 0)
        {
            byte[] pdur = { (byte)(_pdur / 256), (byte)(_pdur - _pdur / 256 * 256) };
            S7Comm s7Comm = new S7Comm(messageType, pdur, s7CommParameters, s7CommDatas, classEror, error);
            Cotp cotp = new CotpDtData();
            S7CommPacker packer = new S7CommPacker(3);
            _pdur++;
            _networkInterface.SendPacket(packer.Pack(cotp, s7Comm));
        }

        public void RecievePacket(byte[] packet)
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
            packet = buffer;
            S7Comm? s7Comm = S7Comm.ParsePacket(packet, out buffer);
            if (s7Comm == null)
            {
                return;
            }
            _pdur = (ushort)(s7Comm.PDUR[0] * 256 + s7Comm.PDUR[1]);
            OnPacketReceived.Invoke(s7Comm);
        }
    }
}
