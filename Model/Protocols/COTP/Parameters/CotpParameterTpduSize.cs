namespace Protocols.COTP.Parameters
{
    class CotpParameterTpduSize : CotpParameter
    {
        public readonly byte _power;

        public CotpParameterTpduSize(uint value)
        {
            if ((value & (value - 1)) != 0)
            {
                throw new ArgumentException("The value must be a power of two");
            }
            _power = 0;
            for (; value > 1;)
            {
                _power++;
                value = value >> 1;
            }
        }

        public override byte[] GetBytes()
        {
            return new byte[] { 192, 1, _power };
        }
    }
}
