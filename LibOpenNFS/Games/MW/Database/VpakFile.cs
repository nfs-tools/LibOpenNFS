using System;

namespace LibOpenNFS.Games.MW.Database
{
    /// <summary>
    /// An intermediate class for VPAK file entries.
    /// </summary>
    public class VpakFile
    {
        /// <summary>
        /// The internal header structure.
        /// </summary>
        public FileHeader FileHeader { get; }
        
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string Name { get; set; }

        public VpakFile(FileHeader fileHeader, string name)
        {
            FileHeader = fileHeader;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}