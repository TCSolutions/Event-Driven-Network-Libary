using NetworkLib.Structures;
using System.Runtime.InteropServices;

namespace NetworkLib.Packets
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PacketHeader
    {
        public int PacketType;
        public PacketFlags Flags;
        internal int DataLength;
        internal int FooterOffset;
    }
}
