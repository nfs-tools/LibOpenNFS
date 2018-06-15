using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class EaInt8 : VltType
    {
        /// <summary>
        /// The field value
        /// </summary>
        public sbyte Value;
        
        public override void Read(BinaryReader br)
        {
            Value = br.ReadSByte();
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