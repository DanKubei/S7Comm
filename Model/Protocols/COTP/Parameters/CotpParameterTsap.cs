namespace Protocols.COTP.Parameters
{
    class CotpParameterTsap : CotpParameter
    {
        public readonly byte[] _value;

        public CotpParameterTsap(byte[] value)
        {
            if (value.Length != 2)
            {
                throw new ArgumentException("Wrong length of argument");
            }
            _value = value;
        }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(2);
            bytes.AddRange(_value);
            return bytes.ToArray();
        }
    }
}
