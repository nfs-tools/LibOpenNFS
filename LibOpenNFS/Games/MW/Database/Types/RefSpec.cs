using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class RefSpec : VltType
    {
        private uint _refClassHash;
        private uint _refCollectionHash;
        
        public override void Read(BinaryReader br)
        {
            _refClassHash = br.ReadUInt32();
            _refCollectionHash = br.ReadUInt32();
            br.ReadUInt32();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => $"0x{_refClassHash:X8} -> 0x{_refCollectionHash:X8}";
    }
}