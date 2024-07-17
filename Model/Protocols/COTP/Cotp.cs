namespace Protocols.COTP
{
    abstract class Cotp : Protocol
    {
        public abstract override byte[] GetBytes();

        public static Cotp? ParsePacket(byte[] packet, out byte[] payload)
        {
            if (packet[0] > packet.Length)
            {
                payload = packet;
                return null;
            }
            switch (packet[1])
            {
                case 240:
                    return CotpDtData.ParsePacket(packet, out payload);
                case 208:
                    return CotpConnectionRequest.ParsePacket(packet, out payload);
                    break;
                default:
                    payload = packet;
                    return null;
            }
        }
    }
}
