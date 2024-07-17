namespace Protocols.S7.Parameters
{
    class S7CommParameterReadVar : S7CommParameterVar
    {
        public S7CommParameterReadVar(byte itemSpecification, byte syntheticId, byte transportLength,
            byte[] length, byte[] databaseIndex, byte valueArea, byte[] itemAddress) :
            base(itemSpecification, syntheticId, transportLength, length, databaseIndex, valueArea,
                itemAddress) { }

        public S7CommParameterReadVar() { }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(base.GetBytes());
            bytes.Insert(0, 4);
            return bytes.ToArray();
        }
    }
}
