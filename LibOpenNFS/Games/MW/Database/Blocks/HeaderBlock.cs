using System;
using System.IO;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database.Blocks
{
    public class HeaderBlock : VltBlockContainer
    {
        private uint _nameCount;
        
        public uint[] IdValues;
        public string[] Names;
        
        public override void Read(BinaryReader reader)
        {
            _nameCount = reader.ReadUInt32();
            IdValues = new uint[_nameCount];
            Names = new string[_nameCount];

            var nameOffsets = new int[_nameCount];

            for (var i = 0; i < _nameCount; ++i)
            {
                IdValues[i] = reader.ReadUInt32();
            }

            for (var i = 0; i < _nameCount; ++i)
            {
                nameOffsets[i] = reader.ReadInt32();
            }

            var position = reader.BaseStream.Position;

            for (var i = 0; i < _nameCount; ++i)
            {
                reader.BaseStream.Seek(position + nameOffsets[i], SeekOrigin.Begin);

                Names[i] = BinaryUtil.ReadNullTerminatedString(reader);
                
                Console.WriteLine(Names[i]);
            }
        }
    }
}