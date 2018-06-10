using System.IO;
using System.Text;
using LibOpenNFS.Utils;

namespace LibOpenNFS
{
    /// <summary>
    /// Utility class to determine the game based on an EXE file.
    /// </summary>
    public static class GameFinder
    {
        /// <summary>
        /// EAGL::SymbolEntry
        /// </summary>
        private const string EaglSymbolEntry = "EAGL::SymbolEntry";

        /// <summary>
        /// EAGL4::SymbolEntry
        /// </summary>
        private const string Eagl4SymbolEntry = "EAGL4::SymbolEntry";

        /// <summary>
        /// NeedForSpeedUndercover
        /// </summary>
        private const string NfsUndercoverString = "NeedForSpeedUndercover";

        #region EAGL Symbol Entry

        /// <summary>
        /// Underground 2 EAGL::SymbolEntry address
        /// </summary>
        private const int EaglSmybolEntryUG2 = 0x003C0620;

        /// <summary>
        /// Most Wanted EAGL4::SymbolEntry address
        /// </summary>
        private const int Eagl4SmybolEntryMW = 0x00495708;

        /// <summary>
        /// Carbon EAGL4::SymbolEntry address
        /// </summary>
        private const int Eagl4SmybolEntryCarbon = 0x005C9FE8;

        /// <summary>
        /// ProStreet EAGL4::SymbolEntry address
        /// </summary>
        private const int Eagl4SmybolEntryProstreet = 0x00571350;

        /// <summary>
        /// World EAGL:SymbolEntry address
        /// </summary>
        private const int EaglSymbolEntryWorld = 0x007DF4B8;

        #endregion

        private const int NfsUndercoverStringAddress = 0x0080ECD8;

        /// <summary>
        /// Returns a <see cref="Game"/> value by using the SymbolEntry address of the given EXE.
        /// </summary>
        /// <param name="exePath"></param>
        /// <returns></returns>
        public static NFSGame GetGame(string exePath)
        {
            DebugUtil.EnsureCondition(File.Exists(exePath), () => $"Can't find file: {exePath}");

            var exeBytes = File.ReadAllBytes(exePath);

            var positions = BinaryUtil.SearchBytePattern(
                Encoding.ASCII.GetBytes(EaglSymbolEntry), exeBytes);

            foreach (var item in positions)
            {
                switch (item)
                {
                    case EaglSmybolEntryUG2:
                        return NFSGame.Underground2;
                    case EaglSymbolEntryWorld:
                        return NFSGame.World;
                    default:
                        return NFSGame.Unknown;
                }
            }

            positions = BinaryUtil.SearchBytePattern(
                Encoding.ASCII.GetBytes(Eagl4SymbolEntry), exeBytes);

            foreach (var item in positions)
            {
                switch (item)
                {
                    case Eagl4SmybolEntryMW:
                        return NFSGame.MW;
                    case Eagl4SmybolEntryCarbon:
                        return NFSGame.Carbon;
                    case Eagl4SmybolEntryProstreet:
                        return NFSGame.ProStreet;
                    default:
                        return NFSGame.Unknown;
                }
            }

            positions = BinaryUtil.SearchBytePattern(
                Encoding.ASCII.GetBytes(NfsUndercoverString), exeBytes);

            foreach (var item in positions)
            {
                switch (item)
                {
                    case NfsUndercoverStringAddress:
                        return NFSGame.Undercover;
                    default:
                        return NFSGame.Unknown;
                }
            }

            return NFSGame.Unknown;
        }
    }
}