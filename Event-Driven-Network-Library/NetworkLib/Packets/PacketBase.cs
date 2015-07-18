using System;
using System.Runtime.InteropServices;
using NetworkLib.Extensions;
using NetworkLib.Encryption;

namespace NetworkLib.Packets
{
    public abstract class PacketBase<T>
    {
        public virtual PacketHeader Header { get; set; }
        public virtual T Data { get; set; }
        public virtual Nullable<PacketFooter> Footer { get; set; }
        public EncryptionType EncryptionMethod = Networking.Core.DefaultEncryption;

        protected abstract byte[] DataToByteArray();
        protected abstract void DataFromByteArray(byte[] bData);

        protected PacketBase() { }
        protected PacketBase(byte[] bData)
        {
            FromExisting(bData);
        }

        public byte[] ToByteArray()
        {
            byte[] bData = DataToByteArray();

            if(EncryptionMethod != EncryptionType.None)
            {
                PacketFooter pFooter = new PacketFooter();
                pFooter.EncryptionMethod = EncryptionMethod;
                PacketEncryption.EncryptionMethods[(int)EncryptionMethod].EncryptPacketData(ref bData, ref pFooter);
                Footer = pFooter;
            }

            byte[] bFooter = MarshalExtensions.StructureToByteArray(Footer);

            PacketHeader pHeader = Header;
            pHeader.FooterOffset = (Footer.HasValue ? bData.Length : -1);

            byte[] dataArray = bData;
            dataArray = dataArray.Append(bFooter);

            pHeader.DataLength = dataArray.Length;

            byte[] bHeader = MarshalExtensions.StructureToByteArray(pHeader);
           

            byte[] retArray = bHeader;
            retArray = retArray.Append(bData);
            retArray = retArray.Append(bFooter);

            return retArray;
        }

        public void FromExisting(byte[] bData)
        {
            byte[] bHeader = new byte[Marshal.SizeOf(Header)];
            byte[] bRemaining = new byte[bData.Length - bHeader.Length];

            Array.Copy(bData, 0, bHeader, 0, bHeader.Length);
            Array.Copy(bData, bHeader.Length, bRemaining, 0, bRemaining.Length);

            PacketHeader pHeader = MarshalExtensions.ByteArrayToStructure<PacketHeader>(bHeader);
            FromExisting(pHeader, bRemaining);
        }

        public void FromExisting(PacketHeader pHeader, byte[] bData)
        {
            Header = pHeader;
            byte[] bActualData = bData;

            if (pHeader.FooterOffset == -1) Footer = null;
            else
            {
                byte[] bFooter = new byte[Marshal.SizeOf(typeof(PacketFooter))];
                Array.Copy(bData, pHeader.FooterOffset, bFooter, 0, bFooter.Length);
                Array.Resize(ref bActualData, pHeader.FooterOffset);
                Footer = MarshalExtensions.ByteArrayToStructure<PacketFooter>(bFooter);

                if (Footer.Value.EncryptionMethod != EncryptionType.None)
                {
                    EncryptionMethod = Footer.Value.EncryptionMethod;
                    PacketEncryption.EncryptionMethods[(int)EncryptionMethod].DecryptPacketData(ref bActualData, Footer.Value);
                }
            }

            DataFromByteArray(bActualData);
        }

        public static implicit operator byte[](PacketBase<T> pBase)
        {
            return pBase.ToByteArray();
        }
    }    
}
