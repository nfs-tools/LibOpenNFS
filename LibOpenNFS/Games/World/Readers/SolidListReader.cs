using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LibOpenNFS.Bundles;
using LibOpenNFS.Bundles.Resources;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.World.Readers
{
    public class SolidListReader : ReadContainer<SolidList>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FileInfo
        {
            private readonly uint blank1;
            private readonly uint blank2;
            private readonly uint tagA;
            private readonly uint tagB;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 56)]
            public readonly string Path;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public readonly string Section;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            private readonly uint[] unknownData;

            private readonly uint unkVarA;
            private readonly uint unkVarB;
            private readonly uint unkVarC;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            private readonly uint[] unknownData2;
        }

        public SolidListReader(BinaryReader reader, long? containerSize) : base(reader, containerSize)
        {
        }

        public override SolidList Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _solidList = new SolidList();

            ReadChunks(ContainerSize);

            return _solidList;
        }

        private void ReadChunks(long totalSize)
        {
            var runTo = Reader.BaseStream.Position + totalSize;

            for (var i = 0; i < 0xFFFF && Reader.BaseStream.Position < runTo; i++)
            {
                var chunkId = Reader.ReadUInt32();
                var chunkSize = Reader.ReadUInt32();

                BinaryUtil.ApplyPadding(Reader, ref chunkSize);

                //Console.WriteLine($"\tID: 0x{chunkId:X8} size: {chunkSize}");
                var chunkRunTo = Reader.BaseStream.Position + chunkSize;

                switch (chunkId)
                {
                    case 0x80134001: // Root
                        {
                            ReadChunks(chunkSize);
                            break;
                        }
                    case 0x80134010: // Object
                        {
                            new SolidObjectReader(Reader, chunkSize).Get();

                            break;
                        }
                    case 0x00134002: // Info
                        {
                            var fileInfo = BinaryUtil.ReadStruct<FileInfo>(Reader);

                            break;
                        }
                    case 0x00134003: // Hash table
                        {
                            DebugUtil.EnsureCondition(
                                chunkSize % 8 == 0,
                                () => $"{chunkSize} % 8 != 0");

                            for (var j = 0; j < chunkSize / 8; j++)
                            {
                                var hash = Reader.ReadUInt32();

                                Reader.ReadUInt32();
                            }

                            break;
                        }
                    default: break;
                }

                Reader.BaseStream.Seek(chunkRunTo, SeekOrigin.Begin);
            }
        }

        private SolidList _solidList;
    }
}