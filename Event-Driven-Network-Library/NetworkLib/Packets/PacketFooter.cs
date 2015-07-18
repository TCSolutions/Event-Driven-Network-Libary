using System.Runtime.InteropServices;

namespace NetworkLib.Packets
{
    [StructLayout(LayoutKind.Sequential, Size = 52)]
    public struct PacketFooter
    {
        [MarshalAs(UnmanagedType.I4)]
        public Encryption.EncryptionType EncryptionMethod;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] EncryptionKey;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Seed;
    }
}
