using System;
using System.Collections.Generic;
using System.IO;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Bundles
{
    public struct BundleReadOptions
    {
        /// <summary>
        /// The location to start from.
        /// </summary>
        public long StartPosition { get; set; }

        /// <summary>
        /// The location to end at.
        /// </summary>
        public long EndPosition { get; set; }
    }

    /// <summary>
    /// Base class for reading chunk bundles.
    /// </summary>
    public abstract class BundleReader : IDisposable
    {
        /// <summary>
        /// Initialize the chunk reader.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="options"></param>
        protected BundleReader(string file, BundleReadOptions? options)
        {
            DebugUtil.EnsureCondition(File.Exists(file), () => $"File not found: {file}");

            Reader = new BinaryReader(File.OpenRead(file));
            Options = options ?? new BundleReadOptions
            {
                StartPosition = -1,
                EndPosition = -1
            };
        }

        /// <summary>
        /// Read chunks from the file.
        /// </summary>
        public List<BundleResource> Read()
        {
            CheckCompression();

            var runTo = Reader.BaseStream.Length;

            if (Options.StartPosition != -1 && Options.EndPosition != -1)
            {
                runTo = Options.EndPosition;
                Reader.BaseStream.Position = Options.StartPosition;
            }

            for (var i = 0; i < 0xFFFF && Reader.BaseStream.Position < runTo; i++)
            {
                var chunkId = Reader.ReadUInt32();
                var chunkSize = Reader.ReadUInt32();
                var chunkRunTo = Reader.BaseStream.Position + chunkSize;

                Console.WriteLine($"ID: 0x{chunkId:X8} size: {chunkSize}");

                switch (chunkId)
                {
                    case 0xB3300000: // BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS
                    {
                        HandleTexturePack(chunkSize);
                        break;
                    }
                    case 0x80134000: // BCHUNK_SPEED_ESOLID_LIST_CHUNKS
                    {
                        HandleSolidList(chunkSize);
                        break;
                    }
                    default: break;
                }

                Reader.BaseStream.Seek(chunkRunTo, SeekOrigin.Begin);
            }

            return Resources;
        }

        public void Dispose()
        {
            Reader?.Dispose();
        }

        /// <summary>
        /// Read a texture pack chunk.
        /// </summary>
        protected virtual void HandleTexturePack(uint size)
        {
        }
        
        /// <summary>
        /// Read a solid list chunk.
        /// </summary>
        protected virtual void HandleSolidList(uint size)
        {
        }

        /// <summary>
        /// Check to see if the file needs to be decompressed.
        /// If so, decompress it.
        /// </summary>
        private void CheckCompression()
        {
            var flag = Reader.ReadChars(4);

            Reader.BaseStream.Position -= 4;

            if (flag[0] == 'J' && flag[1] == 'D' && flag[2] == 'L' && flag[3] == 'Z')
            {
                var allData = Reader.ReadBytes((int) Reader.BaseStream.Length);
                var decompressed = JDLZ.Decompress(allData);

                Reader = new BinaryReader(new MemoryStream(decompressed));
            }
        }

        protected BinaryReader Reader;
        protected BundleReadOptions Options;
        protected List<BundleResource> Resources = new List<BundleResource>();
    }
}