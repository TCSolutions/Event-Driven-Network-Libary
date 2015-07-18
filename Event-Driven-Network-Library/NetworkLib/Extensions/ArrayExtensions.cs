using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLib.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] Append<T>(this T[] tArray, T[] tData)
        {
            T[] retArray = new T[tArray.Length + tData.Length];
            tArray.CopyTo(retArray, 0);
            tData.CopyTo(retArray, tArray.Length);

            return retArray;
        }
    }
}
