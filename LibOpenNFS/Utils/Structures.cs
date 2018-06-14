using System.Runtime.InteropServices;

namespace LibOpenNFS.Utils
{
    public static class Structures
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Point3D
        {
            public readonly float X;
            
            public readonly float Y;
            
            public readonly float Z;
            
            public readonly float W;

            public override string ToString()
            {
                return $"({X}, {Y}, {Z} [{W}])";
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Matrix
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public readonly float[] Data;
        }
    }
}