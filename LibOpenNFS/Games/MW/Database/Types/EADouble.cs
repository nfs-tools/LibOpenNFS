using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class EaDouble : VltType
    {
        public double Value;
        
        public override void Read(BinaryReader br)
        {
            Value = br.ReadDouble();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => Value.ToString();
    }
}