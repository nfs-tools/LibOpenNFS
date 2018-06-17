using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class CollectionKey : VltType
    {
        public uint CollectionHash;

        public override void Read(BinaryReader br)
        {
            CollectionHash = br.ReadUInt32();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => $"CK: 0x{CollectionHash:X8}";
    }
}