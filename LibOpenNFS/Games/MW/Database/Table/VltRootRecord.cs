using System.IO;

namespace LibOpenNFS.Games.MW.Database.Table
{
    public class VltRootRecord : VltRecord
    {
        private int Unknown1 { get; set; }
        private int Unknown2 { get; set; }
        
        // ???
        public int NumEntries { get; set; }
        
        public int Position { get; set; }
        
        private uint Spacer { get; set; }
        
        // ???
        public int[] Hashes { get; set; }
        
        public override void Read(BinaryReader br)
        {
            Unknown1 = br.ReadInt32();
            Unknown2 = br.ReadInt32();
            NumEntries = br.ReadInt32();

            Position = (int) br.BaseStream.Position;

            Spacer = br.ReadUInt32();

            Hashes = new int[NumEntries];

            for (var i = 0; i < NumEntries; ++i)
            {
                Hashes[i] = br.ReadInt32();
            }
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}