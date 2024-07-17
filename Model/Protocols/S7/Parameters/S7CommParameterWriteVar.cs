namespace Protocols.S7.Parameters
{
    class S7CommParameterWriteVar : S7CommParameterVar
    {
        public S7CommParameterWriteVar(byte itemSpecification, byte syntheticId, byte transportLength,
            byte[] length, byte[] databaseIndex, byte valueArea, byte[] itemAddress) :
            base(itemSpecification, syntheticId, transportLength, length, databaseIndex, valueArea,
                itemAddress)
        { }

        public S7CommParameterWriteVar() { }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(base.GetBytes());
            bytes.Insert(0, 5);
            return bytes.ToArray();
        }
    }
}
