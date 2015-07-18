using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkLib.Encryption
{
    public interface IPacketEncryptor
    {
        void EncryptPacketData(ref byte[] Data, ref Packets.PacketFooter pFooter);
        void DecryptPacketData(ref byte[] Data, Packets.PacketFooter pFooter);
    }
}
