using System.IO;
using LibOpenNFS.Games.MW.Database.Table;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    /// <inheritdoc />
    /// <summary>
    /// A VLT table entry. This contains hash, type, and some other stuff.
    /// </summary>
    public class VltTableEntry : IBinReadWrite
    {
        public uint Hash;

        public EntryType EntryType;

        private int _empty;

        private int _empty2;

        public int Address;
        
        public void Read(BinaryReader br)
        {
            Hash = br.ReadUInt32();
            EntryType = (EntryType) br.ReadUInt32();
            _empty = br.ReadInt32();

            // Not sure how this works yet
            _empty2 = _empty != 0 ? _empty : br.ReadInt32();
            Address = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}