using System.IO;

namespace LibOpenNFS.Games.MW.Database
{
    public abstract class VltBlockContainer
    {
        public VltBlock Block { get; set; }

        public abstract void Read(BinaryReader reader);
    }
}