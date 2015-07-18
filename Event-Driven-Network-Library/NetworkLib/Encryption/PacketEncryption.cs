using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkLib.Encryption
{
    public static class PacketEncryption
    {
        public static IPacketEncryptor[] EncryptionMethods
        {
            get
            {
                return new[]
                {
                    new AESPacketEncryptor()
                };
            }
        }
    }
}
