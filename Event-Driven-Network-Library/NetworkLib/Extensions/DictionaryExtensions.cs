using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetworkLib.Extensions
{
    public static class DictionaryExtensions
    {
        public static byte[] Serialize(this Dictionary<String, String> Dictionary)
        {
            using (MemoryStream sStream = new MemoryStream())
            {
                BinaryWriter bWriter = new BinaryWriter(sStream);
                bWriter.Write(Dictionary.Count);
                foreach (var Entry in Dictionary)
                {
                    bWriter.Write(Entry.Key);
                    bWriter.Write(Entry.Value);
                }
                bWriter.Flush();
                return sStream.ToArray();
            }        
        }

        public static Dictionary<String, String> DeSerialize(this byte[] Data)  
        {
            using (MemoryStream sStream = new MemoryStream(Data))
            {
                BinaryReader bReader = new BinaryReader(sStream);
                int len = bReader.ReadInt32();
                var Dictionary = new Dictionary<String, String>(len);
                for (int n = 0; n < len; n++)
                {
                    var key = bReader.ReadString();
                    var value = bReader.ReadString();
                    Dictionary.Add(key, value);
                }
                return Dictionary;  
            }  
        }
    }
}
