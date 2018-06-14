using System;
using System.Collections.Generic;
using System.IO;
using LibOpenNFS.Games.MW.Database.Blocks;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    /// <summary>
    /// Class to read a VPAK file.
    /// </summary>
    public class FileReader : IDisposable
    {
        private readonly VpakFile _file;
        private readonly List<VltBlock> _blocks;
        
        private BinaryReader _vltReader;
        private BinaryReader _binReader;

        private readonly Dictionary<uint, string> _hashDictionary;

        /// <summary>
        /// Initialize the reader
        /// </summary>
        /// <param name="file"></param>
        public FileReader(VpakFile file)
        {
            _file = file;
            _blocks = new List<VltBlock>();
            _hashDictionary = new Dictionary<uint, string>();
        }

        /// <summary>
        /// Read and parse the file data.
        /// </summary>
        /// <param name="reader"></param>
        public void Read(BinaryReader reader)
        {
            reader.BaseStream.Position = _file.FileHeader.VaultLocation;
            
            _vltReader = new BinaryReader(
                new MemoryStream(reader.ReadBytes(_file.FileHeader.VaultLength)));

            reader.BaseStream.Position = _file.FileHeader.BinLocation;
            
            _binReader = new BinaryReader(
                new MemoryStream(reader.ReadBytes(_file.FileHeader.BinLength)));
            
            InternalVaultRead();
            InternalBinRead();
        }

        /// <summary>
        /// Internal VLT read function
        /// </summary>
        private void InternalVaultRead()
        {
            VltBlock block;

            while ((block = ReadBlock(_vltReader)) != null)
            {
                _blocks.Add(block);
                
                Console.WriteLine($"block type {block.Type} @ 0x{block.Position:X8} ({block.BlockLength} bytes)");
            }
        }

        /// <summary>
        /// Internal bin read function
        /// </summary>
        private void InternalBinRead()
        {
            var block = ReadBlock(_binReader);

            DebugUtil.EnsureCondition(
                block.Type == VltType.BinMagic,
                () => $"Expected BinMagic, got {block.Type}"
            );
            
            block.SeekToDataStart(_binReader.BaseStream);

            var runTo = _binReader.BaseStream.Position + block.DataSize();

            while (_binReader.BaseStream.Position < runTo)
            {
                var text = BinaryUtil.ReadNullTerminatedString(_binReader);

                if (text.Length > 0)
                {
                    var hash = JenkinsHash.getHash32(text);

                    if (!_hashDictionary.ContainsKey(hash))
                    {
                        _hashDictionary.Add(hash, text);
                        
                        Console.WriteLine($"0x{hash:X8} -> {text}");
                    }
                }
            }
        }

        /// <summary>
        /// Find the block with the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private VltBlock FindBlock(VltType type) => _blocks.Find(b => b.Type == type);
        
        private VltBlock ReadBlock(BinaryReader reader)
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
            {
                return null;
            }
            
            var block = new VltBlock
            {    
                Position = reader.BaseStream.Position,
                Type = (VltType) reader.ReadInt32(),
                BlockLength = reader.ReadInt32(),
            };
            
            if (!block.IsBlank())
            {
                var vltType = block.Type;
                
                VltBlockContainer bc;

                switch (vltType)
                {
                    case VltType.VltMagic:
                        bc = new HeaderBlock();
                        break;
                    case VltType.TableStart:
                        bc = new TableStartBlock();
                        break;
                    default:
                        bc = new PlaceholderBlock();
                        break;
                }

                bc.Block = block;
                bc.Read(reader);
                block.SeekToNextBlock(reader.BaseStream);

                return block;
            }
            
            return null;
        }

        public void Dispose()
        {
            _vltReader?.Dispose();
            _binReader?.Dispose();
        }
    }
}