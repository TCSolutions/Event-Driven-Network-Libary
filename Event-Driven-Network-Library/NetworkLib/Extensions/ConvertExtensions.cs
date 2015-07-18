using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLib.Packets;

namespace NetworkLib.Extensions
{
    public static class ConvertExtensions
    {
        public static bool ConvertPacketBase(object sourceBase, ref object targetBase)
        {
            if (sourceBase.GetType().BaseType != typeof(PacketBase<>)) return false;
            if (targetBase.GetType().BaseType != typeof(PacketBase<>)) return false;

            targetBase.GetType().GetMethod("FromByteArray").Invoke(targetBase, new object[]{sourceBase.GetType().GetMethod("ToByteArray").Invoke(sourceBase, new object[0])});

            return true;
        }
    }
}
