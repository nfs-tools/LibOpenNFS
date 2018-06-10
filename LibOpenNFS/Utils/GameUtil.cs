using System.IO;

namespace LibOpenNFS.Utils
{
    /// <summary>
    /// Game utilities.
    /// </summary>
    public static class GameUtil
    {
        /// <summary>
        /// Determine what game is contained within a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static NFSGame GetGameFromPath(string path)
        {
            foreach (var game in GameDatabase.SupportedGames)
            {
                var fileName = game.ExectuableFileName;
                var final = Path.Combine(path, fileName);

                if (File.Exists(final))
                {
                    return GameFinder.GetGame(final);
                }
            }

            return NFSGame.Unknown;
        }
    }
}