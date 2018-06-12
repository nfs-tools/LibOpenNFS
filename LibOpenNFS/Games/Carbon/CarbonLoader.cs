using LibOpenNFS.Interfaces;

namespace LibOpenNFS.Games.Carbon
{
    /// <inheritdoc />
    /// <summary>
    /// A loader for Need for Speed: Carbon.
    /// </summary>
    public class CarbonLoader : IGameLoader
    {
        public void Initialize(string directory)
        {
            _directory = directory;
        }

        public void LoadMapStream()
        {
            throw new System.NotImplementedException();
        }

        public void LoadGlobal()
        {
            throw new System.NotImplementedException();
        }

        public void LoadDatabase()
        {
            throw new System.NotImplementedException();
        }

        public void LoadFile(string file)
        {
            throw new System.NotImplementedException();
        }

        private string _directory;
    }
}