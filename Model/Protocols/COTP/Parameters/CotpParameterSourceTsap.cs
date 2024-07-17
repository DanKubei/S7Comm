namespace Protocols.COTP.Parameters
{
    internal class CotpParameterSourceTsap : CotpParameterTsap
    {
        public CotpParameterSourceTsap(byte[] value) : base(value) { }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(193); // pdu type
            bytes.AddRange(base.GetBytes());
            return bytes.ToArray();
        }
    }
}
