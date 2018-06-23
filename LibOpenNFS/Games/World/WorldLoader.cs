using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibOpenNFS.Interfaces;

namespace LibOpenNFS.Games.World
{
    public class WorldLoader : IGameLoader
    {
        public void Initialize(string directory)
        {
            _directory = directory;
        }

        public void LoadMapStream()
        {
            throw new NotImplementedException();
        }

        public void LoadGlobal()
        {
            throw new NotImplementedException();
        }

        public void LoadDatabase()
        {
            throw new NotImplementedException();
        }

        public void LoadFile(string file)
        {
            new WorldBundleReader(file, null).Read();
        }

        private string _directory;
    }
}
