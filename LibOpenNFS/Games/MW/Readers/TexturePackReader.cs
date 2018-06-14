using System;
using System.IO;
using System.Runtime.InteropServices;
using LibOpenNFS.Bundles;
using LibOpenNFS.Bundles.Resources;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Readers
{
    public class TexturePackReader : ReadContainer<TexturePack>
    {
        /// <summary>
        /// The TPK header structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Header
        {
            private readonly uint Marker;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x1C)]
            public readonly string Name;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public readonly string Path;

            public readonly uint Hash;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18)]
            private readonly byte[] Empty;
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TpkTextureHeader
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xC)]
            private readonly byte[] zero;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public readonly string Name;

            public readonly uint TextureHash;

            public readonly uint TypeHash;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            private readonly byte[] blankOne;

            public readonly uint DataOffset;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            private readonly byte[] blankTwo;

            public readonly uint DataSize;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private readonly byte[] blankThree;

            public readonly short Width;

            public readonly short Height;

            private readonly short MipMapLow;

            public readonly short MipMap;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            private readonly byte[] restOfData;
        }
        
        public TexturePackReader(BinaryReader reader, long? containerSize) : base(reader, containerSize)
        {
        }

        public override TexturePack Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }
            
            _texturePack = new TexturePack();

            ReadChunks(ContainerSize);
            
            return _texturePack;
        }

        private void ReadChunks(long totalSize)
        {
            var runTo = Reader.BaseStream.Position + totalSize;

            for (var i = 0; i < 0xFFFF && Reader.BaseStream.Position < runTo; i++)
            {
                var chunkId = Reader.ReadUInt32();
                var chunkSize = Reader.ReadUInt32();
                var chunkRunTo = Reader.BaseStream.Position + chunkSize;
                
                Console.WriteLine($"\tID: 0x{chunkId:X8} size: {chunkSize}");

                switch (chunkId)
                {
                    case 0xB3310000: // TPK root
                    case 0xB3320000: // TPK data root
                    {
                        ReadChunks(chunkSize);
                        break;
                    }
                    case 0x33310001: // TPK header
                    {
                        var header = BinaryUtil.ReadStruct<Header>(Reader);
                        
                        _texturePack.Name = header.Name;
                        _texturePack.Path = header.Path;
                        _texturePack.Hash = header.Hash;
                        
                        break;
                    }
                    case 0x33310002: // TPK hash table
                    {
                        DebugUtil.EnsureCondition(
                            chunkSize % 8 == 0,
                            () => $"{chunkSize} % 8 != 0");
                        
                        var numEntries = chunkSize / 8;

                        for (var j = 0; j < numEntries; j++)
                        {
                            var hash = Reader.ReadUInt32();
                            
                            _texturePack.Hashes.Add(hash);

                            Reader.ReadUInt32();
                        }
                        
                        break;
                    }
                    case 0x33310004: // TPK entry table
                    {
                        DebugUtil.EnsureCondition(
                            chunkSize % 124 == 0,
                            () => $"{chunkSize} % 124 != 0");

                        var tpkHeaders = BinaryUtil.ReadList<TpkTextureHeader>(Reader, chunkSize);

                        foreach (var tpkHeader in tpkHeaders)
                        {
                            Console.WriteLine($"{tpkHeader.Name} {tpkHeader.Width}x{tpkHeader.Height} @ {tpkHeader.DataOffset} (size: {tpkHeader.DataSize})");
                        }
                        
                        break;
                    }
                }
                
                Reader.BaseStream.Seek(chunkRunTo, SeekOrigin.Begin);
            }
        }

        private TexturePack _texturePack;
    }
}