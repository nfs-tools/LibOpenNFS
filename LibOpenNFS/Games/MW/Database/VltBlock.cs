using System.IO;

namespace LibOpenNFS.Games.MW.Database
{
    public class VltBlock
    {
        public VltType Type { get; set; }
        
        public int BlockLength { get; set; }
        
        public long Position { get; set; }

        public int DataSize() => BlockLength - 8;
        
        public bool IsBlank() => BlockLength < 8;

        public void SeekToNextBlock(Stream stream) => stream.Seek(
            Position + BlockLength, 
            SeekOrigin.Begin
        );
        
        public void SeekToDataStart(Stream stream) => stream.Seek(
            Position + 8L, 
            SeekOrigin.Begin
        );
    }
}