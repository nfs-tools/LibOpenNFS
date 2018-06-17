using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class Blob : VltType
    {
        public uint Length;
        public uint Offset;

        public override void Read(BinaryReader br)
        {
            Length = br.ReadUInt32();
            Offset = br.ReadUInt32();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Length} bytes @ 0x{Offset:X8}";
        }
    }
}