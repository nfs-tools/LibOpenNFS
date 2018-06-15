using System;
using System.IO;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database.Table
{
    public class VltRowRecord : VltRecord
    {
        public class Row : IBinReadWrite
        {
            public uint Hash { get; set; }

            public int Position { get; set; }

            private uint Spacer { get; set; }

            private short Unknown1 { get; set; }

            private byte Flag1 { get; set; }

            private byte Flag2 { get; set; }

            /// <summary>
            /// Not sure what this *actually* is, just a guess.
            /// </summary>
            /// <returns></returns>
            public bool IsInVlt() => (Flag1 & 32) != 0 || (Flag1 & 64) != 0;

            public void Read(BinaryReader br)
            {
                Hash = br.ReadUInt32();
                
                Position = (int) br.BaseStream.Position;
                Spacer = br.ReadUInt32();
                Unknown1 = br.ReadInt16();
                Flag1 = br.ReadByte();
                Flag2 = br.ReadByte();
            }

            public void Write(BinaryWriter bw)
            {
                throw new System.NotImplementedException();
            }
        }

        public uint Hash { get; set; }

        public uint Unknown1 { get; set; }

        public uint Unknown2 { get; set; }

        public int Unknown3 { get; set; }

        private int Unknown4 { get; set; }

        private int Unknown5 { get; set; }

        private ushort Unknown6 { get; set; }

        private ushort Unknown7 { get; set; }

        public int Position { get; set; }

        private uint Spacer { get; set; }

        private uint[] UnkArray1 { get; set; }
        
        public Row[] Rows { get; set; }

        public override void Read(BinaryReader br)
        {
            Hash = br.ReadUInt32();
            Unknown1 = br.ReadUInt32();
            Unknown2 = br.ReadUInt32();
            Unknown3 = br.ReadInt32();
            Unknown4 = br.ReadInt32();
            Unknown5 = br.ReadInt32();
            Unknown6 = br.ReadUInt16();
            Unknown7 = br.ReadUInt16();

            Position = (int) br.BaseStream.Position;
            Spacer = br.ReadUInt32();

            UnkArray1 = new uint[Unknown6];

            for (var i = 0; i < Unknown6; ++i)
            {
                UnkArray1[i] = br.ReadUInt32();
            }

            Rows = new Row[Unknown3];

            for (var i = 0; i < Unknown3; ++i)
            {
                Rows[i] = new Row();
                Rows[i].Read(br);
                
                Console.WriteLine($"Row: 0x{Rows[i].Hash:X8} @ 0x{Rows[i].Position:X8}");
            }
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}