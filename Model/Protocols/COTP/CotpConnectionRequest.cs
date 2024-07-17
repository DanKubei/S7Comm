using Protocols.COTP.Parameters;
using Utils;

namespace Protocols.COTP
{
    class CotpConnectionRequest : Cotp
    {
        public readonly byte[] DestinationReference, SourceReferance;
        public readonly byte CotpClass;
        public readonly CotpParameter[] Parameters;

        public CotpConnectionRequest(byte[] destinationReference, byte[] sourceReferance, byte cotpClass, CotpParameter[] parameters)
        {
            if (destinationReference.Length != 2 || sourceReferance.Length != 2)
            {
                throw new ArgumentException("Wrong length of reference argument");
            }
            DestinationReference = destinationReference;
            SourceReferance = sourceReferance;
            CotpClass = cotpClass;
            Parameters = parameters;
        }

        public static CotpConnectionRequest? ParsePacket(byte[] packet, out byte[] payload)
        {
            byte length = packet[0];
            if (length < 7) // Default length without parameters
            {
                payload = packet;
                return null;
            }
            byte[] destinationReference = { packet[2], packet[3] };
            byte[] sourceReference = { packet[4], packet[5] };
            byte cotpClass = packet[6];
            if (length == 7)
            {
                payload = packet.RemoveRange(0, 7);
                return new CotpConnectionRequest(destinationReference, sourceReference, cotpClass, []);
            }
            int parameterPointer = 7 + 1;
            List<CotpParameter> parameters = new List<CotpParameter>();
            while (true)
            {
                if (length <= parameterPointer + 1)
                {
                    payload = packet;
                    return null;
                }
                switch (packet[parameterPointer])
                {
                    case 192:
                        if (packet[parameterPointer + 1] != 1)
                        {
                            payload = packet;
                            return null;
                        }
                        parameters.Add(new CotpParameterTpduSize(packet[parameterPointer + 2]));
                        parameterPointer += 3;
                        break;
                    case 193:
                        if (packet[parameterPointer + 1] != 2)
                        {
                            payload = packet;
                            return null;
                        }
                        parameters.Add(new CotpParameterTpduSize(packet[parameterPointer + 2]));
                        parameterPointer += 4;
                        break;
                    case 194:
                        if (packet[parameterPointer + 1] != 2)
                        {
                            payload = packet;
                            return null;
                        }
                        parameters.Add(new CotpParameterTpduSize(packet[parameterPointer + 2]));
                        parameterPointer += 4;
                        break;
                    default:
                        payload = packet;
                        return null;
                }
                if (parameterPointer == length)
                {
                    break;
                }
            }
            payload = packet.RemoveRange(0, length);
            return new CotpConnectionRequest(destinationReference, sourceReference, cotpClass, parameters.ToArray());
        }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(224); // pdu type
            bytes.AddRange(DestinationReference);
            bytes.AddRange(SourceReferance);
            bytes.Add(CotpClass);
            foreach (CotpParameter parameter in Parameters)
            {
                bytes.AddRange(parameter.GetBytes());
            }
            bytes.Insert(0, (byte)bytes.Count);
            return bytes.ToArray();
        }
    }
}
