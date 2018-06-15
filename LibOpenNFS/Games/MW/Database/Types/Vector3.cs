using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class Vector3 : VltType
    {
        public float X;
        public float Y;
        public float Z;
        
        public override void Read(BinaryReader br)
        {
            X = br.ReadSingle();
            Y = br.ReadSingle();
            Z = br.ReadSingle();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
        
        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}