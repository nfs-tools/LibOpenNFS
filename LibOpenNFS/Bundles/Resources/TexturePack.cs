using System.Collections.Generic;

namespace LibOpenNFS.Bundles.Resources
{
    public class Texture
    {
        public uint Width { get; set; }
        
        public uint Height { get; set; }
        
        public uint TextureHash { get; set; }
        
        public uint TypeHash { get; set; }
        
        public uint DataSize { get; set; }
        
        public byte[] Data { get; set; }
    }
    
    public class TexturePack : BundleResource
    {
        /// <summary>
        /// The name of the texture pack.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The virtual path of the .tpk file.
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// The hash of the texture pack.
        /// </summary>
        public uint Hash { get; set; }
        
        /// <summary>
        /// The list of texture hashes.
        /// </summary>
        public List<uint> Hashes { get; } = new List<uint>();
        
        /// <summary>
        /// The list of textures.
        /// </summary>
        public List<Texture> Textures { get; } = new List<Texture>();
    }
}