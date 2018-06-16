using System;
using System.Linq;
using LibOpenNFS.Bundles;
using LibOpenNFS.Games.MW.Database;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW
{
    public class MWDatabaseReader : DatabaseReader
    {
        public MWDatabaseReader(string file, bool bigEndian = false) : base(file)
        {
            _isBigEndian = bigEndian;
        }

        public override void Read()
        {
            var header = BinaryUtil.ReadStruct<VpakHeader>(Reader);
            
            Console.WriteLine("VPAK:");
            Console.WriteLine($"\tFiles:       {header.FileCount}");
            Console.WriteLine($"\tFile Table @ 0x{header.FileTableLocation:X8}");

            var files = new VpakFile[header.FileCount];
            
            for (var i = 0; i < header.FileCount; ++i)
            {
                var fileHeader = BinaryUtil.ReadStruct<FileHeader>(Reader, 20);

                files[i] = new VpakFile(fileHeader, $"Unnamed File #{i + 1}");
                
                Console.WriteLine($"File #{i + 1} @ 0x{files[i].FileHeader.BinLocation:X8} ({files[i].FileHeader.BinLength}) / 0x{files[i].FileHeader.VaultLocation:X8} ({files[i].FileHeader.VaultLength})");
            }

            Reader.BaseStream.Position = header.FileTableLocation;

            for (var i = 0; i < header.FileCount; ++i)
            {
                files[i].Name = BinaryUtil.ReadNullTerminatedString(Reader);
                
                Console.WriteLine($"File #{i + 1}: {files[i].Name}");
            }
            
            foreach (var file in files)
            {
                Console.WriteLine($"Reading: {file}");
                new FileReader(file).Read(Reader);
            }
        }

        private bool _isBigEndian;
    }
}