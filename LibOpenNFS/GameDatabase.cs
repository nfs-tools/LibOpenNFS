using System.Collections.Generic;

namespace LibOpenNFS
{
    /// <summary>
    /// Contains the list of supported games.
    /// </summary>
    public static class GameDatabase
    {
        public static readonly List<Game> SupportedGames = new List<Game>
        {
            new Game(Game.NfsUG2Id, "Need for Speed: Underground 2", "speed2.exe"),
            new Game(Game.NfsMWId, "Need for Speed: Most Wanted", "speed.exe"),
            new Game(Game.NfsCarbonId, "Need for Speed: Carbon", "nfsc.exe"),
            new Game(Game.NfsProstreetId, "Need for Speed: ProStreet", "nfs.exe"),
            new Game(Game.NfsUndercoverId, "Need for Speed: Undercover", "nfs.exe"),
            new Game(Game.NfsWorldId, "Need for Speed: World", "nfsw.exe"),
        };
    }
}