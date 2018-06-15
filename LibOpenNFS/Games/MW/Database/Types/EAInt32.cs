using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class EaInt32 : VltType
    {
        /// <summary>
        /// The field value
        /// </summary>
        public int Value;
        
        public override void Read(BinaryReader br)
        {
            Value = br.ReadInt32();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}