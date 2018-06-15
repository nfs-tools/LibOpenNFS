using System;
using System.IO;

namespace LibOpenNFS.Games.MW.Database.Table
{
    public class VltClassRecord : VltRecord
    {
        public uint Hash { get; set; }

        private int Unknown1 { get; set; }

        public int NumFields { get; set; }

        public int Position { get; set; }

        private uint Spacer { get; set; }

        private int Unknown2 { get; set; }
        private int Unknown3 { get; set; }
        private int Unknown4 { get; set; }
        private int Unknown5 { get; set; }

        public override void Read(BinaryReader br)
        {
            Hash = br.ReadUInt32();
            Unknown1 = br.ReadInt32();
            NumFields = br.ReadInt32();

            Position = (int) br.BaseStream.Position;

            Spacer = br.ReadUInt32();

            Unknown2 = br.ReadInt32();
            Unknown3 = br.ReadInt32();
            Unknown4 = br.ReadInt32();
            Unknown5 = br.ReadInt32();     
            
            Console.WriteLine($"Class: hash=0x{Hash:X8}, numFields={NumFields}, unk={Unknown1}/{Unknown2}/{Unknown3}/{Unknown4}/{Unknown5}");
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}