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
        private const string FullPrecision = "0.############################################################";

        /// <summary>
        /// Extension method for <see cref="BinaryReader"/> to read an array of an arbitrary type.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="count"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] ReadArray<T>(this BinaryReader reader, int count)
        {
            var result = new T[count];

            for (var i = 0; i < count; ++i)
            {
                result[i] = ReadStruct<T>(reader);
            }
            
            return result;
        }

        /// <summary>
        /// Read a big-endian float.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static float ReadFloatBE(this BinaryReader reader)
        {
            return BitConverter.ToSingle(reader.ReadBytes(4), 0);
        }
        
        /// <summary>
        /// Read a big-endian 64-bit integer.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static long ReadInt64BE(this BinaryReader reader)
        {
            return BitConverter.ToInt64(reader.ReadBytes(8), 0);
        }
        
        /// <summary>
        /// Read a big-endian 32-bit integer.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static int ReadInt32BE(this BinaryReader reader)
        {
            return BitConverter.ToInt32(reader.ReadBytes(4), 0);
        }
        
        /// <summary>
        /// Read a big-endian 16-bit integer.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static short ReadInt16BE(this BinaryReader reader)
        {
            return BitConverter.ToInt16(reader.ReadBytes(2), 0);
        }
        
        /// <summary>
        /// Read a big-endian unsigned 64-bit integer.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ulong ReadUInt64BE(this BinaryReader reader)
        {
            return BitConverter.ToUInt64(reader.ReadBytes(8), 0);
        }
        
        /// <summary>
        /// Read a big-endian unsigned 32-bit integer.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static uint ReadUInt32BE(this BinaryReader reader)
        {
            return BitConverter.ToUInt32(reader.ReadBytes(4), 0);
        }
        
        /// <summary>
        /// Read a big-endian unsigned 16-bit integer.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ushort ReadUInt16BE(this BinaryReader reader)
        {
            return BitConverter.ToUInt16(reader.ReadBytes(2), 0);
        }
        
        /// <summary>
        /// Formats a float value as a string, with full precision.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string FormatFloat(float f)
        {
            return f.ToString(FullPrecision);
        }
        
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
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadNullTerminatedString(BinaryReader reader)
        {
            var str = new StringBuilder();
            char ch;
            while ((ch = (char) reader.ReadByte()) != 0)
                str.Append(ch);
            return str.ToString();
        }

        /// <summary>
        /// Read a structure from a binary file.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="size"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadStruct<T>(BinaryReader reader, int size = 0)
        {
            var bytes = reader.ReadBytes(size == 0 ? Marshal.SizeOf(typeof(T)) : size);

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
        /// Read padding bytes from a stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="outSize"></param>
        public static void ApplyPadding(BinaryReader reader, ref uint outSize)
        {
            uint pad = 0;

            while (reader.ReadByte() == 0x11)
            {
                pad++;
            }

            reader.BaseStream.Seek(pad % 2 == 0 ? -1 : -2, SeekOrigin.Current);

            outSize -= pad % 2 == 0 ? pad : pad - 1;
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