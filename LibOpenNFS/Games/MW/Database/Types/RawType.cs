using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class RawType : VltType
    {
        public int Length;
        
        public override void Read(BinaryReader br)
        {
            br.BaseStream.Seek(Length, SeekOrigin.Current);
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}