using System;
using System.IO;
using System.Reflection;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    public abstract class VltType : IBinReadWrite
    {
        public uint Address { get; set; }

        public bool IsVlt { get; set; }

        public uint TypeHash { get; set; }

        public uint Hash { get; set; }

        public int Index { get; set; }

        public VltInfo Info { get; set; }

        public int Size { get; set; }

        public static VltType Create(Type type)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);
            return constructor?.Invoke(null) as VltType;
        }

        public abstract void Read(BinaryReader br);

        public abstract void Write(BinaryWriter bw);
        
        public override string ToString()
        {
            return "Base type has no representation";
        }
    }
}