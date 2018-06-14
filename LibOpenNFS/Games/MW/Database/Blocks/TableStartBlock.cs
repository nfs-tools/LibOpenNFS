using System;
using System.IO;

namespace LibOpenNFS.Games.MW.Database.Blocks
{
    public class TableStartBlock : VltBlockContainer
    {
        public VltTableEntry[] TableEntries;
        
        public override void Read(BinaryReader reader)
        {
            var numEntries = reader.ReadInt32();
            
            TableEntries = new VltTableEntry[numEntries];

            for (var i = 0; i < numEntries; ++i)
            {
                TableEntries[i] = new VltTableEntry();
                TableEntries[i].Read(reader);
                
                Console.WriteLine($"entry #{i + 1}: type={TableEntries[i].EntryType} ({((int)TableEntries[i].EntryType):X8}), hash=0x{TableEntries[i].Hash:X8}, address={TableEntries[i].Address}");
            }
        }
    }
}