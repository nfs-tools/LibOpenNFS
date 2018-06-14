using System.Runtime.InteropServices;

namespace LibOpenNFS.Games.MW.Database
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileHeader
    {
        public readonly int FileNumber;
        public readonly int BinLength;
        public readonly int VaultLength;
        public readonly int BinLocation;
        public readonly int VaultLocation;
    }
}