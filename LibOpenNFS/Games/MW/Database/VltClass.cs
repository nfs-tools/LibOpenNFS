using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibOpenNFS.Games.MW.Database.Blocks;
using LibOpenNFS.Games.MW.Database.Table;
using LibOpenNFS.Games.MW.Database.Types;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    /// <summary>
    /// This is big. Really big.
    /// </summary>
    public class VltClass : IComparable<VltClass>, IEnumerable
    {
        /// <summary>
        /// The field manager for the class
        /// </summary>
        public class FieldManager : IEnumerable<VltInfo>
        {
            private readonly VltClass _vltClass;
            private readonly List<VltInfo> _infoList;

            public FieldManager(VltClass vltClass)
            {
                _vltClass = vltClass;
                _infoList = new List<VltInfo>();
            }

            /// <summary>
            /// Find an info record by its hash
            /// </summary>
            /// <param name="hash"></param>
            /// <returns></returns>
            public VltInfo Find(uint hash) => _infoList.Find(i => i.RowRecord.Hash == hash);

            public void Init(VltRowRecord rowRecord, TableEndBlock block, BinaryReader vltReader, BinaryReader binReader)
            {
                var info = new VltInfo(_vltClass.ClassRecord.NumFields);
                
                DebugUtil.EnsureCondition(
                    block.UnknownDictionary.ContainsKey(rowRecord.Position),
                    () => "Uh oh.");

                var basePosition = block.UnknownDictionary[rowRecord.Position].Address2;
                
                info.BlockContainer = block;
                info.Class = _vltClass;
                info.RowRecord = rowRecord;

                for (var i = 0; i < _vltClass.ClassRecord.NumFields; ++i)
                {
                    var field = _vltClass.Fields[i];
                    BinaryReader br;

                    if (!field.UnknownMeaning())
                    {
                        Console.WriteLine(@"!UnknownMeaning");
                        br = binReader;
                        br.BaseStream.Seek(basePosition + field.Unknown2, SeekOrigin.Begin);
                    }
                    else
                    {
                        br = null;

                        foreach (var row in rowRecord.Rows)
                        {
                            if (row.Hash == field.Hash)
                            {
                                if (row.IsInVlt())
                                {
                                    Console.WriteLine(@"IsInVlt = true");
                                    br = vltReader;
                                    br.BaseStream.Seek(row.Position, SeekOrigin.Begin);
                                }
                                else
                                {
                                    Console.WriteLine("Not in VLT");
                                    br = binReader;
                                    br.BaseStream.Seek(block.UnknownDictionary[row.Position].Address2,
                                        SeekOrigin.Begin);
                                }
                            }
                        }

                        if (br == null)
                        {
                            continue;
                        }
//                        br.BaseStream.Seek(block.UnknownDictionary[rowRecord.Position].Address2, SeekOrigin.Begin);
                    }

                    var type = VltTypeMap.Instance.GetTypeForKey(field.TypeHash);

                    if (type == null)
                    {
                        type = typeof(RawType);
                    }

                    var vltType = VltType.Create(type);
                    vltType.Size = field.Length;

                    if (vltType is RawType rt)
                    {
                        rt.Length = field.Length;
                    }

                    vltType.Address = (uint) br.BaseStream.Position;
                    vltType.IsVlt = br == vltReader;
                    vltType.TypeHash = field.TypeHash;
                    vltType.Hash = field.Hash;
                    vltType.Info = info;
                    vltType.Read(br);

                    if (!(vltType is RawType))
                    {
                        Console.WriteLine($"Class: 0x{_vltClass.Hash:X8} | Field: 0x{field.Hash:X8} | {vltType.GetType().FullName} -> {vltType}");
                    }
                    
                    info.Set(i, vltType);
                }
                
                _infoList.Add(info);
            }

            public IEnumerator<VltInfo> GetEnumerator()
            {
                return _infoList.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class ClassManager : IEnumerable<VltClass>
        {
            public class ManagedClass
            {
                public uint Hash { get; set; }
                
                public string Value { get; set; }
                
                public int Unknown { get; set; }
            }

            private Dictionary<uint, ManagedClass> _managedClasses;

            public Dictionary<uint, VltClass> Classes { get; private set; }
            
            public void Init(VltRootRecord rootRecord, TableEndBlock teb, BinaryReader br)
            {
                var position = teb.UnknownDictionary[rootRecord.Position].Address2;

                br.BaseStream.Seek(position, SeekOrigin.Begin);
                
                _managedClasses = new Dictionary<uint, ManagedClass>(rootRecord.NumEntries);

                for (var i = 0; i < rootRecord.NumEntries; ++i)
                {
                    var mc = new ManagedClass
                    {
                        Value = BinaryUtil.ReadNullTerminatedString(br),
                        Unknown = rootRecord.Hashes[i]
                    };
                    
                    mc.Hash = JenkinsHash.getHash32(mc.Value);
                    
                    _managedClasses.Add(mc.Hash, mc);
                }
                
                Classes = new Dictionary<uint, VltClass>();
            }

            public void Init(VltClassRecord classRecord, TableEndBlock teb, BinaryReader br)
            {
                var vc = new VltClass();
                vc.Init(classRecord, teb, br);
                Classes.Add(vc.Hash, vc);
            }
            
            public IEnumerator<VltClass> GetEnumerator()
            {
                return Classes.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// The field structure for a VLT class
        /// </summary>
        public class VltClassField : IBinReadWrite
        {
            public uint Hash { get; set; }

            public uint TypeHash { get; set; }

            public ushort Unknown2 { get; set; }

            public ushort Length { get; set; }

            public short Count { get; set; }

            public byte Flag1 { get; set; }

            public byte Flag2 { get; set; }

            public bool IsArray() => (Flag1 & 1) != 0;

            public bool UnknownMeaning() => (Flag1 & 2) == 0;

            public int UnknownUse() => 1 << Flag2;

            public void Read(BinaryReader br)
            {
                Hash = br.ReadUInt32();
                TypeHash = br.ReadUInt32();
                Unknown2 = br.ReadUInt16();
                Length = br.ReadUInt16();
                Count = br.ReadInt16();
                Flag1 = br.ReadByte();
                Flag2 = br.ReadByte();
            }

            public void Write(BinaryWriter bw)
            {
                throw new NotImplementedException();
            }
        }

        public uint Hash { get; private set; }

        public VltBlockContainer Block { get; private set; }

        public VltClassRecord ClassRecord { get; private set; }

        public VltClassField[] Fields { get; private set; }

        public void Init(VltClassRecord classRecord, VltBlockContainer blockContainer, BinaryReader br)
        {
            ClassRecord = classRecord;
            Block = blockContainer;
            Hash = classRecord.Hash;

            if (blockContainer is TableEndBlock teb)
            {
                var position = teb.UnknownDictionary[classRecord.Position].Address2;

                br.BaseStream.Seek(position, SeekOrigin.Begin);

                Fields = new VltClassField[ClassRecord.NumFields];

                for (var i = 0; i < ClassRecord.NumFields; ++i)
                {
                    Fields[i] = new VltClassField();
                    Fields[i].Read(br);
                }
            }
        }

        public int FindFieldIndex(uint hash) => Array.IndexOf(Fields, Fields.First(f => f.Hash == hash));

        public int CompareTo(VltClass other) => Hash.CompareTo(other.Hash);

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}