using Utils;

namespace Protocols.COTP
{
    class CotpDtData : Cotp
    {
        public CotpDtData() { }

        public static CotpDtData? ParsePacket(byte[] packet, out byte[] payload)
        {
            payload = packet.RemoveRange(0, packet[0] + 1);
            return new CotpDtData();
        }

        public override byte[] GetBytes()
        {
            // length, pdu type, reserved
            return new byte[] { 2, 240, 128 };
        }
    }
}
