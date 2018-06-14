using System.IO;

namespace LibOpenNFS.Utils
{
    /// <summary>
    /// An interface for readable and writable structures.
    /// </summary>
    public interface IBinReadWrite
    {
        void Read(BinaryReader br);

        void Write(BinaryWriter bw);
    }
}