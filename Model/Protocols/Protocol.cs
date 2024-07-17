namespace Protocols
{
    abstract class Protocol
    {
        public abstract byte[] GetBytes();
        public List<bool> GetBits()
        {
            List<bool> bits = new List<bool>();
            foreach (byte b in GetBytes())
            {
                bits.AddRange(GetBits(b));
            }
            return bits;
        }

        public static List<bool> GetBits(byte number)
        {
            List<bool> bits = new List<bool>();
            if (number > 1)
            {
                bits.AddRange(GetBits((byte)(number / 2)));
            }
            bits.Add(number % 2 == 1);
            return bits;
        }
    }
}
