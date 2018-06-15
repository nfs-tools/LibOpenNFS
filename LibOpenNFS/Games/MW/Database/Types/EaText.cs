using System.IO;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class EaText : VltType
    {
        public uint Offset;

        public string Value;
        
        public override void Read(BinaryReader br)
        {
            Offset = br.ReadUInt32();

            if (Offset > br.BaseStream.Length)
            {
                Offset = Address;
            }

            if (Offset == 0)
            {
                Value = "(null)";
            }
            else
            {
                var position = br.BaseStream.Position;
                br.BaseStream.Position = Offset;

                Value = BinaryUtil.ReadNullTerminatedString(br);
                br.BaseStream.Position = position;
            }
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => Value;
    }
}