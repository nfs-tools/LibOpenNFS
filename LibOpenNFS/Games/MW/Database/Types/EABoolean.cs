using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class EaBoolean : VltType
    {
        public bool Value;
        
        public override void Read(BinaryReader br)
        {
            Value = br.ReadBoolean();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => Value.ToString();
    }
}