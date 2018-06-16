using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class AxlePair : VltType
    {
        public float Front;
        public float Rear;
        
        public override void Read(BinaryReader br)
        {
            Front = br.ReadSingle();
            Rear = br.ReadSingle();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => $"({Front}, {Rear})";
    }
}