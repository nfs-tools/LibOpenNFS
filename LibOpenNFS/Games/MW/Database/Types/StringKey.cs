using System;
using System.IO;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class StringKey : VltType
    {
        public ulong Hash64;
        public uint Hash32;
        public uint Offset;
        public string Value;

        public override void Read(BinaryReader br)
        {
            Hash64 = br.ReadUInt64();
            Hash32 = br.ReadUInt32();
            Offset = br.ReadUInt32();

            if (Offset == 0u)
            {
                Value = "(null)";
                return;
            }

            if (Offset > br.BaseStream.Length)
            {
                throw new Exception("StringKey offset is out of bounds!");
            }

            var position = br.BaseStream.Position;
            br.BaseStream.Position = Offset;
            Value = BinaryUtil.ReadNullTerminatedString(br);
            br.BaseStream.Position = position;
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => $"0x{Hash32:X8} [{Value}]";
    }
}