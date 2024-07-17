using Protocols;
using Protocols.COTP;

namespace Packers
{
    class CotpPacker
    {
        private byte _tpktVersion;

        public CotpPacker(byte tpktVersion)
        {
            _tpktVersion = tpktVersion;
        }

        public byte[] Pack(Cotp cotp)
        {
            List<byte> result = new List<byte>();
            byte[] cotpPayload = cotp.GetBytes();
            result.AddRange((new TPKT(_tpktVersion, (ushort)(cotpPayload.Length)).GetBytes()));
            result.AddRange(cotpPayload);
            return result.ToArray();
        }
    }
}
