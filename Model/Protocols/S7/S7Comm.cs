using Protocols.S7.Parameters;
using Utils;

namespace Protocols.S7
{
    class S7Comm : Protocol
    {
        public enum MessageType : byte
        {
            Job = 1,
            Ack = 2,
            AckData = 3,
            UserData = 7
        }

        public readonly MessageType messageType;
        public readonly byte[] PDUR;
        public readonly S7CommParameter[] parameters;
        public readonly S7CommData[] Data;
        public readonly byte ClassError;
        public readonly byte Error;

        /* 
         * PDUR - protocol data unit reference
         * Error be only in Ack_Data packets
         */
        public S7Comm(MessageType messageType, byte[] pdur, S7CommParameter[] s7CommParameters, S7CommData[] s7CommData, byte classEror = 0, byte error = 0)
        {
            this.messageType = messageType;
            PDUR = pdur;
            parameters = s7CommParameters;
            Data = s7CommData;
            ClassError = classEror;
            Error = error;
        }

        public static S7Comm? ParsePacket(byte[] packet, out byte[] payload)
        {
            MessageType messageType;
            try
            {
                messageType = (MessageType)packet[1];
            }
            catch (Exception e)
            {
                payload = packet;
                return null;
            }
            if (packet[0] != 32 || packet[2] + packet[3] != 0) // Verify id, message type, reservation
            {
                payload = packet;
                return null;
            }
            byte[] pdur = { packet[4], packet[5] };
            ushort parametersLength = (ushort)(packet[6] * 256 + packet[7]);
            ushort dataLength = (ushort)(packet[8] * 256 + packet[9]);
            if (parametersLength + dataLength != packet.Length - 10)
            {
                payload = packet;
                return null;
            }
            int dataPointer = 10;
            byte classError = 0, error = 0;
            if (messageType == MessageType.AckData)
            {
                classError = packet[10];
                error = packet[11];
                dataPointer += 2;
            }
            S7CommParameter? s7CommParameter = null;
            S7CommData? s7CommData = null;
            if (parametersLength != 0)
            {
                switch (packet[dataPointer])
                {
                    case 240:
                        if (parametersLength == 2)
                        {
                            s7CommParameter = new S7CommParameterSetupCommunication();
                        }
                        if (packet[dataPointer + 1] != 0) 
                        {
                            payload = packet;
                            return null;
                        }
                        s7CommParameter = new S7CommParameterSetupCommunication(
                            [packet[dataPointer + 2], packet[dataPointer + 3]],
                            [packet[dataPointer + 4], packet[dataPointer + 5]],
                            [packet[dataPointer + 6], packet[dataPointer + 7]]
                            );
                        break;
                    case 4:
                        if (parametersLength == 2)
                        {
                            s7CommParameter = new S7CommParameterReadVar();
                        }
                        if (packet[dataPointer + 1] != 1)
                        {
                            payload = packet;
                            return null;
                        }
                        s7CommParameter = new S7CommParameterReadVar(
                            packet[dataPointer + 2], packet[dataPointer + 4], packet[dataPointer + 5],
                            [packet[dataPointer + 6], packet[dataPointer + 7]], [packet[dataPointer + 8], packet[dataPointer + 9]],
                            packet[dataPointer + 10], [packet[dataPointer + 11], packet[dataPointer + 12], packet[dataPointer + 13]]
                            );
                        break;
                    case 5:
                        if (parametersLength == 2)
                        {
                            s7CommParameter = new S7CommParameterWriteVar();
                        }
                        s7CommParameter = new S7CommParameterReadVar(
                            packet[dataPointer + 2], packet[dataPointer + 4], packet[dataPointer + 5],
                            [packet[dataPointer + 6], packet[dataPointer + 7]], [packet[dataPointer + 8], packet[dataPointer + 9]],
                            packet[dataPointer + 10], [packet[dataPointer + 11], packet[dataPointer + 12], packet[dataPointer + 13]]
                            );
                        break;
                }
                dataPointer += parametersLength;
            }
            if (dataLength != 0)
            {
                S7CommData.ResponseCode responseCode;
                try
                {
                    responseCode = (S7CommData.ResponseCode)packet[dataPointer];
                }
                catch (Exception e)
                {
                    payload = packet;
                    return null;
                }
                S7CommData.TransportSize transportSize;
                try
                {
                    transportSize = (S7CommData.TransportSize)packet[dataPointer + 1];
                }
                catch (Exception e)
                {
                    payload = packet;
                    return null;
                }
                int length = packet[dataPointer + 2] * 256 + packet[dataPointer + 3];
                List<byte> data = new List<byte>();
                for (int i = 0; i < length; i++)
                {
                    data.Add(packet[dataPointer + 4 + i]);
                }
                s7CommData = new S7CommData(responseCode, transportSize, data.ToArray());
            }
            payload = packet;
            return new S7Comm(messageType, pdur, [s7CommParameter], [s7CommData], classError, error);
        }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(new byte[] { 0x32, (byte)messageType, 0, 0 });
            if (parameters[0].GetType() == typeof(S7CommParameterSetupCommunication))
            {
                if (messageType == MessageType.Job)
                {
                    bytes.AddRange([0x86, 0x7a]);
                }
                else
                {
                    bytes.AddRange([0x2b, 0x32]);
                }
            }
            else
            {
                bytes.AddRange(PDUR);
            }
            bytes.AddRange([(byte)(parameters.Length / 256), (byte)(parameters.Length - parameters.Length / 256 * 256)]);
            bytes.AddRange([(byte)(Data.Length / 256), (byte)(Data.Length - Data.Length / 256 * 256)]);
            if (messageType == MessageType.AckData)
            {
                bytes.AddRange(new byte[] { ClassError, Error });
            }
            List<byte> buffer = new List<byte>();
            foreach (S7CommParameter parameter in parameters)
            {
                buffer.AddRange(parameter.GetBytes());
            }
            bytes.AddRange(buffer);
            buffer.Clear();
            foreach (S7CommData data in Data)
            {
                buffer.AddRange(data.GetBytes());
            }
            bytes.AddRange(buffer);
            return bytes.ToArray();
        }
    }
}
