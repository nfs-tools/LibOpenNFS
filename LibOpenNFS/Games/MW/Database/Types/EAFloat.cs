using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class EaFloat : VltType
    {
        public float Value;
        
        public override void Read(BinaryReader br)
        {
            Value = br.ReadSingle();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => Value.ToString();
    }
}