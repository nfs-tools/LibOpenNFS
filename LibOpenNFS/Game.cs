namespace LibOpenNFS
{
    public class Game
    {
        public const int NfsUG2Id = 1002;
        public const int NfsMWId = 1003;
        public const int NfsCarbonId = 1004;
        public const int NfsProstreetId = 1005;
        public const int NfsUndercoverId = 1006;
        public const int NfsWorldId = 1007;

        /// <summary>
        /// The ID of this <see cref="Game"/>.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// <see cref="Game"/> title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// <see cref="Game"/> exe file name.
        /// <para>Example: speed.exe</para>
        /// </summary>
        public string ExectuableFileName { get; }

        public Game(int id, string title, string exeFileName)
        {
            Id = id;
            Title = title;
            ExectuableFileName = exeFileName;
        }
    }
}