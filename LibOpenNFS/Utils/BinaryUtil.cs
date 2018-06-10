using System;
using System.Collections.Generic;
using System.Linq;

namespace LibOpenNFS.Utils
{
    /// <summary>
    /// Binary file utilities.
    /// </summary>
    public static class BinaryUtil
    {
        /// <summary>
        /// Search for a byte pattern in an array of bytes.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<int> SearchBytePattern(byte[] pattern, byte[] bytes)
        {
            var positions = new List<int>();
            var patternLength = pattern.Length;
            var totalLength = bytes.Length;
            var firstMatchByte = pattern[0];
            
            for (var i = 0; i < totalLength; i++)
            {
                if (firstMatchByte == bytes[i] && totalLength - i >= patternLength)
                {
                    var match = new byte[patternLength];
                    Array.Copy(bytes, i, match, 0, patternLength);
                    if (match.SequenceEqual<byte>(pattern))
                    {
                        positions.Add(i);
                        i += patternLength - 1;
                    }
                }
            }
            return positions;
        }
    }
}