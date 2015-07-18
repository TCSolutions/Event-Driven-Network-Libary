using System;
using NetworkLib.Packets;
using NetworkLib.Extensions;

namespace ApexNetworkExtensions.Packets
{
    public class MarshallablePacket<T> : PacketBase<T>
    {
        public MarshallablePacket() : base() { }
        public MarshallablePacket(byte[] bData) : base(bData) { }

        protected sealed override byte[] DataToByteArray()
        {
            return MarshalExtensions.StructureToByteArray(Data);
        }

        protected sealed override void DataFromByteArray(byte[] bData)
        {
            Data = MarshalExtensions.ByteArrayToStructure<T>(bData);
        }
    }
}
