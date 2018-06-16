using System.Collections.Generic;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    public static class HashManager
    {
        private static readonly Dictionary<uint, string> Hashes;

        static HashManager()
        {
            Hashes = new Dictionary<uint, string>();
        }

        public static string HashToValue(uint hash)
        {
            return Hashes.ContainsKey(hash) ? Hashes[hash] : $"0x{hash:X8}";
        }

        public static void AddHash(string value)
        {
            var hash = JenkinsHash.getHash32(value);

            if (!Hashes.ContainsKey(hash))
            {
                Hashes[hash] = value;
            }
        }
    }
}