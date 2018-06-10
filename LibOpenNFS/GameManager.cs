using System;
using LibOpenNFS.Games.MW;
using LibOpenNFS.Interfaces;
using LibOpenNFS.Utils;

namespace LibOpenNFS
{
    /// <summary>
    /// The list of supported games.
    /// </summary>
    public enum NFSGame : byte
    {
        Underground2 = 1 << 0,
        MW = 1 << 1,
        Carbon = 1 << 2,
        ProStreet = 1 << 3,
        Undercover = 1 << 4,
        World = 1 << 5,
        
        Unknown = 255
    }
    
    /// <summary>
    /// Handles game loading.
    /// </summary>
    public class GameManager
    {
        private static Lazy<GameManager> Lazy => new Lazy<GameManager>(() => new GameManager());
        
        public static GameManager Instance => Lazy.Value;

        /// <summary>
        /// The current game loader being used.
        /// </summary>
        private IGameLoader _loader;

        /// <summary>
        /// Initialize the game manager.
        /// </summary>
        /// <remarks>private because this class is a singleton.</remarks>
        private GameManager()
        {
        }

        /// <summary>
        /// Load a game.
        /// </summary>
        /// <param name="directory"></param>
        public void LoadGame(string directory)
        {
            var game = GameUtil.GetGameFromPath(directory);

            switch (game)
            {
                case NFSGame.MW:
                {
                    _loader = new MWLoader();
                    _loader.Initialize(directory);
                    
                    break;
                }
                default:
                    throw new Exception($"Unsupported game: {game}");
            }
        }

        public void Reset()
        {
            
        }
    }
}