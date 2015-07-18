using System;

namespace NetworkLib.Packets
{
    public class BytePacket : PacketBase<byte[]>
    {
        public BytePacket() : base() { }
        public BytePacket(byte[] bData) : base(bData) { }

        protected override byte[] DataToByteArray()
        {
            return (Data != null) ? Data : new byte[0];
        }
        protected override void DataFromByteArray(byte[] bData)
        {
            Data = bData;
        }
    }
}
