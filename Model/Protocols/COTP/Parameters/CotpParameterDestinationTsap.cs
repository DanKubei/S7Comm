namespace Protocols.COTP.Parameters
{
    internal class CotpParameterDestinationTsap : CotpParameterTsap
    {
        public CotpParameterDestinationTsap(byte[] value) : base(value) { }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(194); // pdu type
            bytes.AddRange(base.GetBytes());
            return bytes.ToArray();
        }
    }
}
