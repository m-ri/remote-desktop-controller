using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//TODO finire refactoring
namespace CommonClasses.SocketMessages
{
    public static class SerializationMethods
    {
        public static T FromByteArray<T>(byte[] rawValue)
        {
            GCHandle handle = GCHandle.Alloc(rawValue, GCHandleType.Pinned);
            T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }

        public static byte[] ToByteArray(object value, int maxLength)
        {
            int rawsize = Marshal.SizeOf(value);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle =  GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            handle.Free();
            if (maxLength < rawdata.Length)
            {
                byte[] temp = new byte[maxLength];
                Array.Copy(rawdata, temp, maxLength);
                return temp;
            }
            else
            {
                return rawdata;
            }
        }
        //http://stackoverflow.com/questions/22972607/convert-dictionary-or-list-to-byte
        public static Byte[] SerializeToByteArray(object value)
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, value);

            //This gives you the byte array.
            return mStream.ToArray();
        }
        public static object DeserializeFromByteArray(Byte[] bytes)
        {
            var mStream = new MemoryStream(bytes);
            var binDeformatter = new BinaryFormatter();
            return binDeformatter.Deserialize(mStream);
        }
    }
}
