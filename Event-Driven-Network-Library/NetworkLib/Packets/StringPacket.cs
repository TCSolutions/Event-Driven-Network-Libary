using System;
namespace NetworkLib.Packets
{
    public class StringPacket : PacketBase<string>
    {
        public StringPacket() : base() { }
        public StringPacket(byte[] bData) : base(bData) { }

        protected override byte[] DataToByteArray()
        {
            return System.Text.Encoding.UTF8.GetBytes(Data);
        }
        protected override void DataFromByteArray(byte[] bData)
        {
            Data = System.Text.Encoding.UTF8.GetString(bData);
        }
    }
}
