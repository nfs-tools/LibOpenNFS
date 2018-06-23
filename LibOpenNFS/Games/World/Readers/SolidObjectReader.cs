using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using LibOpenNFS.Bundles;
using LibOpenNFS.Bundles.Resources;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.World.Readers
{
    public class SolidObjectReader : ReadContainer<SolidObject>
    {
        /// <summary>
        /// Helper class for managing vertices
        /// </summary>
        internal class VertexStream
        {
            public readonly List<float> Data = new List<float>();
            public uint Position;
            public uint Interval;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct Material
        {
            public readonly int Flags;
            public readonly int ID;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct ObjectHeader
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            private readonly byte[] zero;

            public readonly uint ObjectHash;

            public readonly uint NumTris;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public readonly byte[] pad2;

            public readonly Structures.Point3D MinPoint;
            public readonly Structures.Point3D MaxPoint;
            public readonly Structures.Matrix Matrix;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public readonly uint[] unknown;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 52)]
        internal struct MeshDescriptor
        {
            public readonly long Unknown1;

            public readonly uint Unknown2;

            public readonly uint Flags;

            public readonly uint MaterialShaderNum;

            public readonly uint Zero;

            public readonly uint NumVertexStreams;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public readonly uint[] Zeroes;

            public readonly uint NumTriangles;

            public readonly uint NumTriIndex;

            public readonly uint Zero2;
        }

        public SolidObjectReader(BinaryReader reader, long? containerSize) : base(reader, containerSize)
        {
            _faces = new List<ushort[]>();
            _vertexStreams = new List<VertexStream>();
            _materials = new List<Material>();
        }

        public override SolidObject Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _solidObject = new SolidObject();

            ReadChunks(ContainerSize);
            CompleteProcessing();

            return _solidObject;
        }

        private void ReadChunks(long totalSize)
        {
            var runTo = Reader.BaseStream.Position + totalSize;

            for (var i = 0; i < 0xFFFF && Reader.BaseStream.Position < runTo; i++)
            {
                var chunkId = Reader.ReadUInt32();
                var chunkSize = Reader.ReadUInt32();

                BinaryUtil.ApplyPadding(Reader, ref chunkSize);

                Console.WriteLine($"\tID: 0x{chunkId:X8} size: {chunkSize}");
                var chunkRunTo = Reader.BaseStream.Position + chunkSize;

                switch (chunkId)
                {
                    case 0x00134011:
                        {
                            _header = BinaryUtil.ReadStruct<ObjectHeader>(Reader);

                            //Console.WriteLine($"object 0x{_header.ObjectHash:X8} @ ({_header.Matrix.Data[12]}, {_header.Matrix.Data[13]}, {_header.Matrix.Data[14]})");
                            _name = BinaryUtil.ReadNullTerminatedString(Reader);
                            //Console.WriteLine(_name);

                            break;
                        }
                    case 0x80134100: // mesh header
                        {
                            ReadChunks(chunkSize);
                            break;
                        }
                    case 0x00134900:
                        {
                            var msd = BinaryUtil.ReadStruct<MeshDescriptor>(Reader);

                            //Console.WriteLine(@"MESH INFO:");
                            //Console.WriteLine($"\tFlags -> 0x{msd.Flags:X8}");
                            //Console.WriteLine($"\tMSN -> {msd.MaterialShaderNum}");
                            //Console.WriteLine($"\tNVS -> {msd.NumVertexStreams}");
                            //Console.WriteLine($"\tNT -> {msd.NumTriangles}");
                            //Console.WriteLine($"\tNTI -> {msd.NumTriIndex}");

                            _meshDescriptor = msd;

                            break;
                        }
                    case 0x00134012:
                        {
                            for (var j = 0; j < chunkSize / 8; j++)
                            {
                                var texHash = Reader.ReadUInt32();

                                //Console.WriteLine($"TEXTURE ID: 0x{texHash:X8}");

                                Reader.BaseStream.Position += 4;
                            }

                            break;
                        }
                    case 0x00134C02:
                        {
                            //Console.WriteLine($"MAT NAME: {BinaryUtil.ReadNullTerminatedString(Reader)}");
                            break;
                        }
                    case 0x00134B02:
                        {
                            Debug.Assert(chunkSize % _meshDescriptor.MaterialShaderNum == 0);

                            for (var j = 0; j < _meshDescriptor.MaterialShaderNum; ++j)
                            {
                                //Console.WriteLine($"MATERIAL INFO #{j + 1}");
                                var material = BinaryUtil.ReadStruct<Material>(Reader, 116);
                                //Console.WriteLine($"\tFlags -> 0x{material.Flags:X8}");
                                //Console.WriteLine($"\tHash  -> 0x{material.ID:X8}");
                            }

                            break;
                        }
                    case 0x00134b01:
                    {
                        if (_meshDescriptor.NumVertexStreams != 1) break;

                        var vs = new VertexStream();

                        while (Reader.BaseStream.Position < chunkRunTo)
                        {
                            vs.Data.Add(Reader.ReadSingle());
                            vs.Position += 4;
                        }

                        vs.Interval = chunkSize / _maxVertex / 4;

                        _vertexStreams.Add(vs);

                        break;
                    }
                    case 0x00134b03: // faces
                    {
                        for (var j = 0; j < _header.NumTris; j++)
                        {
                            var f1 = (ushort)(Reader.ReadUInt16() + 1);
                            var f2 = (ushort)(Reader.ReadUInt16() + 1);
                            var f3 = (ushort)(Reader.ReadUInt16() + 1);

                            if (f1 > _maxVertex)
                            {
                                _maxVertex = f1;
                            }

                            if (f2 > _maxVertex)
                            {
                                _maxVertex = f2;
                            }

                            if (f3 > _maxVertex)
                            {
                                _maxVertex = f3;
                            }

                            _faces.Add(new[] { f1, f2, f3 });
                        }

                        break;
                    }
                }

                Reader.BaseStream.Seek(chunkRunTo, SeekOrigin.Begin);
            }
        }

        private void CompleteProcessing()
        {
            if (_vertexStreams.Count == 1)
            {
                using (var writer = new StreamWriter(File.OpenWrite($"{_name}.obj")))
                {
                    writer.WriteLine($"g {_name}");

                    var vs = _vertexStreams[0];
                    var pos = 0;

                    while (pos < vs.Data.Count)
                    {
                        var x = vs.Data[pos];
                        var y = vs.Data[pos + 1];
                        var z = vs.Data[pos + 2];

                        //var xBytes = BitConverter.GetBytes(x);
                        //var yBytes = BitConverter.GetBytes(y);
                        //var zBytes = BitConverter.GetBytes(z);

                        //x = vs.Data[pos] = BinaryUtil.UnpackIfNecessary(x, xBytes, _header.MinPoint.X, _header.MaxPoint.X, false);
                        //y = vs.Data[pos + 1] = BinaryUtil.UnpackIfNecessary(y, yBytes, _header.MinPoint.Y, _header.MaxPoint.Y, false);
                        //z = vs.Data[pos + 2] = BinaryUtil.UnpackIfNecessary(z, zBytes, _header.MinPoint.Z, _header.MaxPoint.Z, false);

                        pos += (int)vs.Interval;

                        writer.WriteLine($"v {BinaryUtil.FormatFloat(x)} {BinaryUtil.FormatFloat(y)} {BinaryUtil.FormatFloat(z)}");
                    }

                    foreach (var ft in _faces)
                    {
                        writer.WriteLine($"f {ft[0]} {ft[1]} {ft[2]}");
                    }
                }
            }
        }

        private readonly List<VertexStream> _vertexStreams;
        private readonly List<Material> _materials;
        private readonly List<ushort[]> _faces;
        private ushort _maxVertex;
        private string _name;

        private ObjectHeader _header;
        private SolidObject _solidObject;
        private MeshDescriptor _meshDescriptor;
    }
}
