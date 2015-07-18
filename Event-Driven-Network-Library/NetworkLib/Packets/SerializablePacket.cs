using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkLib.Packets
{
    public class SerializablePacket<T> : PacketBase<T>
    {
        public SerializablePacket() : base() { }
        public SerializablePacket(byte[] bData) : base(bData) { }

        protected sealed override byte[] DataToByteArray()
        {
            IFormatter IFormat = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Position = 0;
            IFormat.Serialize(ms, Data);
            byte[] retArray = ms.ToArray();
            ms.Close();
            return retArray;
        }

        protected sealed override void DataFromByteArray(byte[] bData)
        {
            IFormatter IFormat = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(bData);
            ms.Position = 0;
            Data = (T)IFormat.Deserialize(ms);
            ms.Close();
        }
    }
}
