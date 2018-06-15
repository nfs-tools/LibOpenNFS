using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class Matrix : VltType
    {
        public float M11; // [1,1]
        public float M12; // [1,2]
        public float M13; // [1,3]
        public float M14; // [1,4]

        public float M21; // [2,1]
        public float M22; // [2,2]
        public float M23; // [2,3]
        public float M24; // [2,4]

        public float M31; // [3,1]
        public float M32; // [3,2]
        public float M33; // [3,3]
        public float M34; // [3,4]

        public float M41; // [4,1]
        public float M42; // [4,2]
        public float M43; // [4,3]
        public float M44; // [4,4]
        
        public override void Read(BinaryReader br)
        {
            M11 = br.ReadSingle();
            M12 = br.ReadSingle();
            M13 = br.ReadSingle();
            M14 = br.ReadSingle();

            M21 = br.ReadSingle();
            M22 = br.ReadSingle();
            M23 = br.ReadSingle();
            M24 = br.ReadSingle();

            M31 = br.ReadSingle();
            M32 = br.ReadSingle();
            M33 = br.ReadSingle();
            M34 = br.ReadSingle();
            
            M41 = br.ReadSingle();
            M42 = br.ReadSingle();
            M43 = br.ReadSingle();
            M44 = br.ReadSingle();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}