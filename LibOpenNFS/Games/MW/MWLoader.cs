using System;
using System.IO;
using LibOpenNFS.Bundles;
using LibOpenNFS.Bundles.Resources;
using LibOpenNFS.Interfaces;
using LibOpenNFS.VFS;

namespace LibOpenNFS.Games.MW
{
    /// <inheritdoc />
    /// <summary>
    /// A loader for Need for Speed: Most Wanted.
    /// </summary>
    public class MWLoader : IGameLoader
    {
        public void Initialize(string directory)
        {
            _directory = directory;
        }

        public void LoadMapStream()
        {
            var bundleFile = Path.Combine(_directory, "TRACKS", "L2RA.BUN");
            var streamFile = Path.Combine(_directory, "TRACKS", "STREAML2RA.BUN");

            var msr = new MWMapStreamReader(
                bundleFile,
                streamFile
            );
            
            msr.Init();
            msr.Read();

            var sections = msr.GetSections();

            foreach (var section in sections)
            {
                var sectionBundle = VfsManager.CreateBundle(section.Name);
                
                Console.WriteLine($"Reading section: {section.Name} @ 0x{section.Offset:X8}");
                
                // Create a new reader
                var reader = new MWBundleReader(streamFile, new BundleReadOptions
                {
                    StartPosition = section.Offset,
                    EndPosition = section.Offset + section.Size
                });
                
                var resources = reader.Read();

                foreach (var resource in resources)
                {
                    if (resource is TexturePack tpk)
                    {
                        sectionBundle.MountResource(VfsManager.CreateTexturePackResource(tpk));
                    }
                }

                VfsManager.Instance
                    .FindBundle(msr.MapStreamId)
                    .MountBundle(sectionBundle);
            }
        }

        public void LoadGlobal()
        {
            throw new System.NotImplementedException();
        }

        public void LoadDatabase()
        {
            new MWDatabaseReader(Path.Combine(_directory, "GLOBAL", "attributes_xbox.bin")).Read();
//            new MWDatabaseReader(Path.Combine(_directory, "GLOBAL", "gameplay.bin")).Read();
        }

        public void LoadFile(string file)
        {
            var bundle = VfsManager.CreateBundle(Path.GetFileName(file));
            var reader = new MWBundleReader(file, null);

            reader.Read();
        }
        
        private string _directory;
    }
}