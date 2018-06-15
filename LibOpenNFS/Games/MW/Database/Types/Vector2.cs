using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class Vector2 : VltType
    {
        public float X;
        public float Y;
        
        public override void Read(BinaryReader br)
        {
            X = br.ReadSingle();
            Y = br.ReadSingle();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => $"({X}, {Y})";
    }
}