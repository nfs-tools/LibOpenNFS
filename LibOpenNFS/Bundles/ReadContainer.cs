using System.IO;

namespace LibOpenNFS.Bundles
{
    /// <summary>
    /// A container used for reading chunk data from a file.
    /// </summary>
    /// <typeparam name="TC"></typeparam>
    public abstract class ReadContainer<TC> where TC : BundleResource
    {
        protected ReadContainer(BinaryReader reader, long? containerSize)
        {
            Reader = reader;

            if (containerSize != null)
            {
                ContainerSize = (long) containerSize;
            }
        }

        public abstract TC Get();

        protected BinaryReader Reader;

        protected long ContainerSize;
    }
}