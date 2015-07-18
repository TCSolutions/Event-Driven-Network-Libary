using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NetworkLib.Extensions
{
    public static class MarshalExtensions
    {
        public static byte[] StructureToByteArray(object structData)
        {
            if (structData == null) return new byte[0];

            IntPtr pStructure = Marshal.AllocHGlobal(Marshal.SizeOf(structData));
            Marshal.StructureToPtr(structData, pStructure, false);

            byte[] bStructData = new byte[Marshal.SizeOf(structData)];

            Marshal.Copy(pStructure, bStructData, 0, bStructData.Length);
            Marshal.FreeHGlobal(pStructure);

            return bStructData;
        }

        public static T ByteArrayToStructure<T>(byte[] structData)
        {
            IntPtr pStructure = Marshal.AllocHGlobal(structData.Length);
            Marshal.Copy(structData, 0, pStructure, structData.Length);

            T oStruct = (T)Marshal.PtrToStructure(pStructure, typeof(T));

            Marshal.FreeHGlobal(pStructure);

            return oStruct;
        }
    }
}
