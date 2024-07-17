namespace Protocols.S7.Parameters
{
    class S7CommParameterVar : S7CommParameter
    {
        public readonly byte _itemSpecification, _syntheticId, _transportLength, _valueArea;
        public readonly byte[]? _length, _databaseIndex, _itemAddress;

        public S7CommParameterVar(byte itemSpecification, byte syntheticId, byte transportLength,
            byte[] length, byte[] databaseIndex, byte valueArea, byte[] itemAddress)
        {
            if (length.Length != 2 || databaseIndex.Length != 2 || itemAddress.Length != 3)
            {
                throw new ArgumentException("Wrong length of arguments");
            }
            _itemSpecification = itemSpecification;
            _syntheticId = syntheticId;
            _transportLength = transportLength;
            _length = length;
            _databaseIndex = databaseIndex;
            _valueArea = valueArea;
            _itemAddress = itemAddress;
        }

        public S7CommParameterVar() { }

        public override byte[] GetBytes()
        {
            if (_length == null)
            {
                return new byte[] { 1 };
            }
            List<byte> bytes = new List<byte>();
            bytes.Add(1); // function code, reservation
            bytes.Add(_itemSpecification);
            bytes.Add(10); // address length
            bytes.Add(_syntheticId);
            bytes.Add(_transportLength);
            bytes.AddRange(_length);
            bytes.AddRange(_databaseIndex);
            bytes.Add(_valueArea);
            bytes.AddRange(_itemAddress);
            return bytes.ToArray();
        }
    }
}
