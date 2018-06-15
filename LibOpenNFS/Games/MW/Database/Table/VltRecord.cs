using System;
using System.IO;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database.Table
{
    public abstract class VltRecord : IBinReadWrite
    {
        public VltTableEntry TableEntry { get; set; }

        public abstract void Read(BinaryReader br);

        public abstract void Write(BinaryWriter bw);

        public static VltRecord GetRecord(EntryType type)
        {
            switch (type)
            {
                case EntryType.Row:
                    return new VltRowRecord();
                case EntryType.Class:
                    return new VltClassRecord();
                case EntryType.Root:
                    return new VltRootRecord();
                default:
                    throw new ArgumentException($"Unknown value: {type}");
            }
        }

        public VltRootRecord AsRoot() => this as VltRootRecord;
        public VltClassRecord AsClass() => this as VltClassRecord;
        public VltRowRecord AsRow() => this as VltRowRecord;
    }
}