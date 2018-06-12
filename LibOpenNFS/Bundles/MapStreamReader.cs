using System;
using System.Collections.Generic;
using System.IO;
using LibOpenNFS.Utils;
using LibOpenNFS.VFS;

namespace LibOpenNFS.Bundles
{
    /// <summary>
    /// A basic section object.
    /// </summary>
    public class Section
    {
        /// <summary>
        /// The name of the section
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The size (in bytes) of the section.
        /// </summary>
        public uint Size { get; set; }
        
        /// <summary>
        /// The section's data offset in the master stream.
        /// </summary>
        public uint Offset { get; set; }
        
        /// <summary>
        /// Other properties of the section.
        /// </summary>
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Base class for reading chunk bundles.
    /// </summary>
    public abstract class MapStreamReader : IDisposable
    {
        private const uint SectionsChunkId = 0x00034110;

        /// <summary>
        /// Initialize the chunk reader.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="streamFile"></param>
        protected MapStreamReader(string file, string streamFile)
        {
            DebugUtil.EnsureCondition(File.Exists(file), () => $"File not found: {file}");
            DebugUtil.EnsureCondition(File.Exists(streamFile), () => $"File not found: {streamFile}");

            Reader = new BinaryReader(File.OpenRead(file));
        }

        /// <summary>
        /// Initialize the reader.
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Get the list of sections.
        /// </summary>
        /// <returns></returns>
        public List<Section> GetSections() => Sections;

        /// <summary>
        /// Read chunks from the file.
        /// </summary>
        public void Read()
        {
            for (var i = 0; i < 0xFFFF && Reader.BaseStream.Position < Reader.BaseStream.Length; i++)
            {
                var chunkId = Reader.ReadUInt32();
                var chunkSize = Reader.ReadUInt32();
                var chunkRunTo = Reader.BaseStream.Position + chunkSize;

                Console.WriteLine($"ID: 0x{chunkId:X8} size: {chunkSize}");

                switch (chunkId)
                {
                    case SectionsChunkId:
                    {
                        ReadSections(chunkSize);
                        break;
                    }
                    default:
                        break;
                }

                Reader.BaseStream.Seek(chunkRunTo, SeekOrigin.Begin);
            }
        }

        public void Dispose()
        {
            Reader?.Dispose();
        }

        /// <summary>
        /// Read the sections chunk and mount each section as a bundle.
        /// </summary>
        protected abstract void ReadSections(uint size);

        protected BinaryReader Reader;
        protected readonly List<Section> Sections = new List<Section>();
    }
}