using Utils;

namespace Protocols
{
    class TPKT : Protocol
    {
        public readonly byte Version;
        public readonly ushort Length;

        public TPKT(byte version, ushort length)
        {
            if (length > 65536 - 4)
            {
                throw new ArgumentException("Impossible length");
            }
            Version = version;
            Length = (ushort)(length + 4);
        }

        public static TPKT? ParsePacket(byte[] packet, out byte[] payload)
        {
            if (packet[1] != 0 || packet[3] != packet.Length)
            {
                payload = packet;
                return null;
            }
            payload = packet.RemoveRange(0, 4);
            return new TPKT(packet[0], (byte)(packet[2] * 256 + packet[3]));
        }

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[4];
            bytes[0] = Version;
            bytes[1] = 0; // reserved
            bytes[2] = (byte)(Length / 256);
            bytes[3] = (byte)(Length - bytes[2] * 256);
            return bytes;
        }
    }
}
