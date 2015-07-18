using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NetworkLib.Structures
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PacketFlags
    {
        [FieldOffset(0)]
        public int FullValue;
        [FieldOffset(0)]
        public short LowPart;
        [FieldOffset(2)]
        public short HighPart;
        [FieldOffset(0)]
        public byte Byte1;
        [FieldOffset(1)]
        public byte Byte2;
        [FieldOffset(2)]
        public byte Byte3;
        [FieldOffset(3)]
        public byte Byte4;

        public static implicit operator PacketFlags(int value)
        {
            PacketFlags pf = new PacketFlags();
            pf.FullValue = value;

            return pf;
        }
    }
}
