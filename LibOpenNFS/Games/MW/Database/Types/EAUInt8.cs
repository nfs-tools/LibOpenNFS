using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class EaUInt8 : VltType
    {
        /// <summary>
        /// The field value
        /// </summary>
        public byte Value;
        
        public override void Read(BinaryReader br)
        {
            Value = br.ReadByte();
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