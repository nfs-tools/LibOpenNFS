using System.IO;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    public class VltRowInfo : IBinReadWrite
    {
        public int Address;
        public short Unknown2;
        public short Unknown3;
        public int Address2;
        
        public void Read(BinaryReader br)
        {
            Address = br.ReadInt32();
            Unknown2 = br.ReadInt16();
            Unknown3 = br.ReadInt16();
            Address2 = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}