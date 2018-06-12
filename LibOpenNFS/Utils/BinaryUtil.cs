using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibOpenNFS.Utils
{
    /// <summary>
    /// Binary file utilities.
    /// </summary>
    public static class BinaryUtil
    {
        /// <summary>
        /// Packed floats: they exist.
        /// They're so weird, that you have to operate on byte arrays to get proper values.
        /// And, additionally, they're quite frustrating to deal with sometimes.
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static unsafe float GetPackedFloat(byte* buf, int pos)
        {
            return (float) ((long) ((short*) buf)[pos]) / (float) 0x8000;
        }

        /// <summary>
        /// Align a value to a given offset. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        public static long Align(long value, int align)
        {
            if (value % align == 0)
            {
                return 0;
            }

            return align - value % align;
        }
        
        /// <summary>
        /// Read a C-style string from a binary file.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadNullTerminatedString(BinaryReader stream)
        {
            var str = new StringBuilder();
            char ch;
            while ((ch = (char) stream.ReadByte()) != 0)
                str.Append(ch);
            return str.ToString();
        }

        /// <summary>
        /// Read a structure from a binary file.
        /// </summary>
        /// <param name="reader"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadStruct<T>(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var theStructure = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }
        
        /// <summary>
        /// Write a structure to a binary file.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public static void WriteStruct<T>(BinaryWriter writer, T instance)
        {
            writer.Write(MarshalStruct(instance));
        }

        /// <summary>
        /// Marshal a structure to a byte array.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static byte[] MarshalStruct<T>(T instance)
        {
            var size = Marshal.SizeOf(instance);
            var arr = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(instance, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        /// <summary>
        /// Repeatedly read a struct of a given type from a binary file into a list.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="size"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> ReadList<T>(BinaryReader reader, long size)
        {
            var boundary = reader.BaseStream.Position + size;
            var items = new List<T>();
            var itemCount = size / Marshal.SizeOf(typeof(T));

            DebugUtil.EnsureCondition(
                reader.BaseStream.Position + itemCount * Marshal.SizeOf(typeof(T)) <= boundary,
                () => $"Cannot read items of type {typeof(T)} from file! Invalid size ({size}) or type?");

            for (var i = 0; i < itemCount; i++)
                items.Add(ReadStruct<T>(reader));

            return items;
        }
        
        /// <summary>
        /// Search for a byte pattern in an array of bytes.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<int> SearchBytePattern(byte[] pattern, byte[] bytes)
        {
            var positions = new List<int>();
            var patternLength = pattern.Length;
            var totalLength = bytes.Length;
            var firstMatchByte = pattern[0];

            for (var i = 0; i < totalLength; i++)
            {
                if (firstMatchByte == bytes[i] && totalLength - i >= patternLength)
                {
                    var match = new byte[patternLength];
                    Array.Copy(bytes, i, match, 0, patternLength);
                    if (match.SequenceEqual(pattern))
                    {
                        positions.Add(i);
                        i += patternLength - 1;
                    }
                }
            }

            return positions;
        }
    }
}