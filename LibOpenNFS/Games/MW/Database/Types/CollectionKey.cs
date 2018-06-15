using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class CollectionKey : VltType
    {
        public uint Hash;
        
        public override void Read(BinaryReader br)
        {
            Hash = br.ReadUInt32();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => $"CK: 0x{Hash:X8}";
    }
}