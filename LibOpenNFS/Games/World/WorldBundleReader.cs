using LibOpenNFS.Bundles;
using LibOpenNFS.Games.World.Readers;

namespace LibOpenNFS.Games.World
{
    public class WorldBundleReader : BundleReader
    {
        public WorldBundleReader(string file, BundleReadOptions? options) : base(file, options)
        {
        }

        protected override void HandleSolidList(uint size)
        {
            Resources.Add(new SolidListReader(Reader, size).Get());
        }

        protected override void HandleTexturePack(uint size)
        {
        }
    }
}
