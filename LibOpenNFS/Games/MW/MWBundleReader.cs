using LibOpenNFS.Bundles;
using LibOpenNFS.Games.MW.Readers;

namespace LibOpenNFS.Games.MW
{
    public class MWBundleReader : BundleReader
    {
        public MWBundleReader(string file, BundleReadOptions? options) : base(file, options)
        {
        }

        protected override void HandleTexturePack(uint size)
        {
            Resources.Add(new TexturePackReader(Reader, size).Get());
        }
    }
}