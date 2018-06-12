using System;
using System.IO;
using LibOpenNFS.Bundles;
using LibOpenNFS.Bundles.Resources;

namespace LibOpenNFS.Games.MW.Readers
{
    public class TexturePackReader : ReadContainer<TexturePack>
    {
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
                }
                
                Reader.BaseStream.Seek(chunkRunTo, SeekOrigin.Begin);
            }
        }

        private TexturePack _texturePack;
    }
}