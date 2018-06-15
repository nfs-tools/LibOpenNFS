using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class EaUInt64 : VltType
    {
        /// <summary>
        /// The field value
        /// </summary>
        public ulong Value;
        
        public override void Read(BinaryReader br)
        {
            Value = br.ReadUInt64();
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