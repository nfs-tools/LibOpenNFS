using System.IO;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Bundles
{
    /// <summary>
    /// Base class for reading VLT databases.
    /// </summary>
    public abstract class DatabaseReader
    {
        /// <summary>
        /// Initialize the database reader.
        /// </summary>
        /// <param name="file"></param>
        protected DatabaseReader(string file)
        {
            DebugUtil.EnsureCondition(File.Exists(file), () => $"File not found: {file}");

            Reader = new BinaryReader(File.OpenRead(file));
        }
        
        /// <summary>
        /// Read the VLT data.
        /// </summary>
        public abstract void Read();
        
        protected BinaryReader Reader;
    }
}