using System;
using System.Linq;
using System.Runtime.InteropServices;
using LibOpenNFS.Bundles;
using LibOpenNFS.Utils;
using LibOpenNFS.VFS;

namespace LibOpenNFS.Games.MW
{
    public class MWMapStreamReader : MapStreamReader
    {
        /// <summary>
        /// The stream section structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SectionStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public readonly string Name;

            public readonly uint StreamChunkNumber;

            public readonly uint Unknown2;

            public readonly uint MasterStreamChunkNumber;

            public readonly uint MasterStreamChunkOffset;

            public readonly uint Size1;

            public readonly uint Size2;

            public readonly uint Size3;

            public readonly uint Unknown3;

            public readonly float X;

            public readonly float Y;

            public readonly float Z;

            public readonly uint Hash;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24)]
            public readonly byte[] RestOfData;
        }

        public MWMapStreamReader(string file, string streamFile) : base(file, streamFile)
        {
        }

        public override void Init()
        {
            var locBundle = VfsManager.CreateBundle("LocationBundle");
            var mapStream = VfsManager.CreateBundle("MapStream");
            
            VfsManager.Instance.MountBundle("/tracks", locBundle);
            VfsManager.Instance.MountBundle("/tracks", mapStream);

            LocBundleId = locBundle.ID;
            MapStreamId = mapStream.ID;
        }

        protected override void ReadSections(uint size)
        {
            var sections = BinaryUtil.ReadList<SectionStruct>(Reader, size);

            foreach (var section in sections)
            {
                Sections.Add(new Section
                {
                    Name = section.Name,
                    Offset = section.MasterStreamChunkOffset,
                    Size = section.Size1
                });
            }
        }

        public Guid LocBundleId { get; private set; }

        public Guid MapStreamId { get; private set; }
    }
}