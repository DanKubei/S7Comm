using Protocols;
using Protocols.COTP;
using Protocols.S7;

namespace Packers
{
    class S7CommPacker
    {
        private byte _tpktVersion;

        public S7CommPacker(byte tpktVersion)
        {
            _tpktVersion = tpktVersion;
        }

        public byte[] Pack(Cotp cotp, S7Comm s7Comm)
        {
            List<byte> result = new List<byte>();
            byte[] cotpPayload = cotp.GetBytes();
            byte[] s7CommPayload = s7Comm.GetBytes();
            result.AddRange((new TPKT(_tpktVersion, (ushort)(cotpPayload.Length + s7CommPayload.Length)).GetBytes()));
            result.AddRange(cotpPayload);
            result.AddRange(s7CommPayload);
            return result.ToArray();
        }
    }
}
