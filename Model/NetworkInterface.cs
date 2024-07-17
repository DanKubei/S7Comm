using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    abstract class NetworkInterface
    {
        public delegate void PacketDelegate(byte[] packet);

        public event PacketDelegate? OnPacketReceived;

        protected void InvokeOnPacketReceived(byte[] packet)
        {
            OnPacketReceived?.Invoke(packet);
        }

        public abstract void SendPacket(byte[] packet);
    }
}
