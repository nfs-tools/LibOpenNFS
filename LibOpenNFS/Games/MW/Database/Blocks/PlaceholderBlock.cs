using System.IO;

namespace LibOpenNFS.Games.MW.Database.Blocks
{
    public class PlaceholderBlock : VltBlockContainer
    {
        public override void Read(BinaryReader reader)
        {
            reader.BaseStream.Seek(Block.DataSize(), SeekOrigin.Current);
        }
    }
}