using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocols.S7
{
    class S7CommData
    {
        public enum ResponseCode : byte
        {
            Success = 255,
            InvalidAddress = 5
        }

        public enum TransportSize : byte
        {
            Bit = 3
        }

        public readonly ResponseCode responseCode;
        public readonly TransportSize transportSize;
        public readonly byte[] data;

        public S7CommData(ResponseCode responseCode, TransportSize transportSize, byte[] data)
        {
            this.responseCode = responseCode;
            this.transportSize = transportSize;
            this.data = data;
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(new byte[] { (byte)responseCode, (byte)transportSize, (byte)(data.Length / 256), (byte)(data.Length - data.Length / 256 * 256) });
            bytes.AddRange(data);
            return bytes.ToArray();
        }
    }
}
