using System;
using System.IO;
using System.Security.Cryptography;

namespace NetworkLib.Encryption
{
    public class AESPacketEncryptor : IPacketEncryptor
    {
        //THIS IS NOT PROPER ENCRYPTION
        //PLEASE DO NOT USE THIS.
        //I BEG
        //LETS JUST PRETEND THIS WAS A TEST FOR THE INTERFACE

        public void EncryptPacketData(ref byte[] Data, ref Packets.PacketFooter pFooter)
        {
            pFooter.EncryptionKey = new byte[32];
            pFooter.Seed = new byte[16];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            
            rng.GetBytes(pFooter.EncryptionKey);
            rng.GetBytes(pFooter.Seed);
            rng.Dispose();

            Data = AESEncryptData(Data, pFooter.EncryptionKey, pFooter.Seed);
        }

        public void DecryptPacketData(ref byte[] Data, Packets.PacketFooter pFooter)
        {
            Data = AESDecryptData(Data, pFooter.EncryptionKey, pFooter.Seed);
        }

        private static byte[] AESEncryptData(byte[] Data, byte[] key, byte[] IVSeed)
        {
            byte[] retByte;
            
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes rfcDBGen = new Rfc2898DeriveBytes(key, IVSeed, 128);
                encryptor.IV = rfcDBGen.GetBytes(16);
                encryptor.Key = rfcDBGen.GetBytes(32);
                rfcDBGen.Dispose();
                using(MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(Data, 0, Data.Length);
                        cs.Close();
                    }
                    retByte = ms.ToArray();
                }
            }

            return retByte;
        }

        private static byte[] AESDecryptData(byte[] Data, byte[] key, byte[] IVSeed)
        {
            byte[] retByte;

            using (Aes decryptor = Aes.Create())
            {
                Rfc2898DeriveBytes rfcDBGen = new Rfc2898DeriveBytes(key, IVSeed, 128);
                decryptor.IV = rfcDBGen.GetBytes(16);
                decryptor.Key = rfcDBGen.GetBytes(32);
                rfcDBGen.Dispose();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(Data, 0, Data.Length);
                        cs.Close();
                    }
                    retByte = ms.ToArray();
                }
            }

            return retByte;
        }
    }
}
