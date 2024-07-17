namespace Protocols.S7.Parameters
{
    class S7CommParameterSetupCommunication : S7CommParameter
    {
        public readonly byte[]? _maxAmqCalling, _maxAmqCalled, _pduLength;

        public S7CommParameterSetupCommunication(byte[] maxAmqCalling, byte[] maxAmqCalled, byte[] pduLength)
        {
            if (maxAmqCalled.Length != 2 || maxAmqCalling.Length != 2 || pduLength.Length != 2)
            {
                throw new ArgumentException("Wrong length of argument");
            }
            _maxAmqCalling = maxAmqCalling;
            _maxAmqCalled = maxAmqCalled;
            _pduLength = pduLength;
        }

        public S7CommParameterSetupCommunication() { }

        public override byte[] GetBytes()
        {
            if (_maxAmqCalling == null)
            {
                return new byte[] { 240, 0 }; // function code, reservation
            }
            List<byte> bytes = new List<byte>();
            bytes.AddRange(new byte[]{ 240, 0 }); // function code, reservation
            bytes.AddRange(_maxAmqCalling);
            bytes.AddRange(_maxAmqCalled);
            bytes.AddRange(_pduLength);
            return bytes.ToArray();
        }
    }
}
