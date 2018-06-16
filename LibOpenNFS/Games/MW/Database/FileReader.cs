using System;
using System.Collections.Generic;
using System.IO;
using LibOpenNFS.Games.MW.Database.Blocks;
using LibOpenNFS.Games.MW.Database.Table;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    /// <summary>
    /// Class to read a VPAK file.
    /// </summary>
    public class FileReader : IDisposable
    {
        private readonly VpakFile _file;
        private readonly List<VltBlockContainer> _blocks;

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
            _blocks = new List<VltBlockContainer>();
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

            if (FindBlock(VltMarker.TableStart) is TableStartBlock tsb
                && FindBlock(VltMarker.TableEnd) is TableEndBlock teb)
            {
                foreach (var t in tsb.TableEntries)
                {
                    switch (t.EntryType)
                    {
                        case EntryType.Root:
                        {
                            VltClass.ClassManager.Instance.Init(t.Record.AsRoot(), teb, _binReader);
                            break;
                        }
                        case EntryType.Class:
                        {
                            VltClass.ClassManager.Instance.Init(t.Record.AsClass(), teb, _binReader);
                            break;
                        }
                        case EntryType.Row:
                        {
                            var rowRecord = t.Record.AsRow();
                            var vltClass = VltClass.ClassManager.Instance.Classes[rowRecord.Unknown1];
                            var fieldManager = new VltClass.FieldManager(vltClass);
                            
                            fieldManager.Init(rowRecord, teb, _vltReader, _binReader);
                            
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        /// <summary>
        /// Internal VLT read function
        /// </summary>
        private void InternalVaultRead()
        {
            VltBlockContainer block;

            while ((block = ReadBlock(_vltReader)) != null)
            {
                _blocks.Add(block);

//                Console.WriteLine(
//                    $"block type {block.Block.Type} @ 0x{block.Block.Position:X8} ({block.Block.BlockLength} bytes)");
            }

            if (FindBlock(VltMarker.TableStart) is TableStartBlock tsb)
            {
                foreach (var t in tsb.TableEntries)
                {
                    t.InitRecord(_vltReader);
                }
            }
        }

        /// <summary>
        /// Internal bin read function
        /// </summary>
        private void InternalBinRead()
        {
            var block = ReadBlock(_binReader);

            DebugUtil.EnsureCondition(
                block.Block.Type == VltMarker.BinMagic,
                () => $"Expected BinMagic, got {block.Block.Type}"
            );

            block.Block.SeekToDataStart(_binReader.BaseStream);

            var runTo = _binReader.BaseStream.Position + block.Block.DataSize();

            while (_binReader.BaseStream.Position < runTo)
            {
                var text = BinaryUtil.ReadNullTerminatedString(_binReader);

                if (text.Length > 0)
                {
                    var hash = JenkinsHash.getHash32(text);

                    if (!_hashDictionary.ContainsKey(hash))
                    {
                        _hashDictionary.Add(hash, text);

//                        Console.WriteLine($"0x{hash:X8} -> {text}");
                    }
                }
            }
        }

        /// <summary>
        /// Find the block with the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private VltBlockContainer FindBlock(VltMarker type) => _blocks.Find(b => b.Block.Type == type);

        private VltBlockContainer ReadBlock(BinaryReader reader)
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
            {
                return null;
            }

            var block = new VltBlock
            {
                Position = reader.BaseStream.Position,
                Type = (VltMarker) reader.ReadInt32(),
                BlockLength = reader.ReadInt32(),
            };

            if (!block.IsBlank())
            {
                var vltType = block.Type;

                VltBlockContainer bc;

                switch (vltType)
                {
                    case VltMarker.VltMagic:
                        bc = new HeaderBlock();
                        break;
                    case VltMarker.TableStart:
                        bc = new TableStartBlock();
                        break;
                    case VltMarker.TableEnd:
                        bc = new TableEndBlock();
                        break;
                    default:
                        bc = new PlaceholderBlock();
                        break;
                }

                bc.Block = block;
                bc.Read(reader);
                block.SeekToNextBlock(reader.BaseStream);

                return bc;
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