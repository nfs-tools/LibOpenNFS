namespace LibOpenNFS.Interfaces
{
    /// <summary>
    /// The base interface for a game loader class.
    /// Game loaders call back to the <see cref="VFS.VfsManager"/> to map bundles and resources.
    /// </summary>
    /// <remarks>A game loader has the following loading modes: MapStream, Global, or Database. Individual file loading is also available.</remarks>
    public interface IGameLoader
    {
        /// <summary>
        /// Initialize the loader and set the game directory.
        /// </summary>
        /// <param name="directory"></param>
        void Initialize(string directory);

        /// <summary>
        /// Loads the game's map stream, located in the TRACKS folder.
        /// </summary>
        void LoadMapStream();

        /// <summary>
        /// Loads the game's global files from the GLOBAL folder.
        /// </summary>
        void LoadGlobal();

        /// <summary>
        /// Load the game's VLT database. Not present in Underground 2.
        /// </summary>
        void LoadDatabase();

        /// <summary>
        /// Load an individual file.
        /// </summary>
        /// <param name="file">The file path relative to the game directory.</param>
        void LoadFile(string file);
    }
}