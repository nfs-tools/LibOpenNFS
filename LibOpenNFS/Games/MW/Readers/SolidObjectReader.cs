using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LibOpenNFS.Bundles;
using LibOpenNFS.Bundles.Resources;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Readers
{
    public class SolidObjectReader : ReadContainer<SolidObject>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ObjectHeader
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

        public SolidObjectReader(BinaryReader reader, long? containerSize) : base(reader, containerSize)
        {
            _faces = new List<ushort[]>();
            _vertexStream = new List<byte>();
            _maxVertex = 0;
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
                    case 0x80134100:
                    {
                        ReadChunks(totalSize);
                        break;
                    }
                    case 0x00134011: // object header
                    {
                        _header = BinaryUtil.ReadStruct<ObjectHeader>(Reader);

                        Console.WriteLine(BinaryUtil.ReadNullTerminatedString(Reader));

                        break;
                    }
                    case 0x00134b01: // vertices
                    {
                        _numVertexChunks++;

                        _vertexStream.AddRange(Reader.ReadBytes((int) chunkSize));
                        
                        break;
                    }
                    case 0x00134b03: // faces
                    {
                        for (var j = 0; j < _header.NumTris; j++)
                        {
                            var f1 = (ushort) (Reader.ReadUInt16() + 1);
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
                    default: break;
                }

                Reader.BaseStream.Seek(chunkRunTo, SeekOrigin.Begin);
            }
        }

        private void CompleteProcessing()
        {
            if (_numVertexChunks != 1)
            {
                Console.Error.WriteLine("Invalid number of vertex chunks. Not doing anything.");
                return;
            }

            //var vertexStride = _vertexStream.Count / _maxVertex;

            if (_vertexStream.Count % _maxVertex == 0)
            {
                var stride = _vertexStream.Count / _maxVertex;

                var bytesRead = 0;
                var verts = new List<float[]>();

                using (var br = new BinaryReader(new MemoryStream(_vertexStream.ToArray())))
                {
                    while (bytesRead < _vertexStream.Count)
                    {
                        var xBytes = br.ReadBytes(4);
                        var yBytes = br.ReadBytes(4);
                        var zBytes = br.ReadBytes(4);

                        var x = BitConverter.ToSingle(xBytes, 0);
                        var y = BitConverter.ToSingle(yBytes, 0);
                        var z = BitConverter.ToSingle(zBytes, 0);

                        x = BinaryUtil.UnpackIfNecessary(x, xBytes, _header.MinPoint.X, _header.MaxPoint.X);
                        y = BinaryUtil.UnpackIfNecessary(y, yBytes, _header.MinPoint.Y, _header.MaxPoint.Y, false);
                        z = BinaryUtil.UnpackIfNecessary(z, zBytes, _header.MinPoint.Z, _header.MaxPoint.Z);

                        verts.Add(new [] { x, y, z });

                        br.BaseStream.Position += stride - 12;

                        bytesRead += stride;
                    }
                }

                using (var writer = new StreamWriter(File.OpenWrite($"obj_0x{_header.ObjectHash:X8}.obj")))
                {
                    writer.WriteLine($"g 0x{_header.ObjectHash:X8}");

                    foreach (var vertex in verts)
                    {
                        writer.WriteLine($"v {vertex[0]} {vertex[1]} {vertex[2]}");
                    }

                    foreach (var face in _faces)
                    {
                        writer.WriteLine($"f {face[0]} {face[1]} {face[2]}");
                    }
                }
            }
            else
            {
                Console.Error.WriteLine("Invalid maxFace. Fix me!!!");
            }
        }

        private readonly List<byte> _vertexStream;
        private readonly List<ushort[]> _faces;

        private ObjectHeader _header;
        private SolidObject _solidObject;
        private int _numVertexChunks;
        private ushort _maxVertex;
    }
}
