using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class ArrayType : VltType, IEnumerable<VltType>
    {
        public short Entries;
        public short MaxEntries;
        private short _length;

        public List<VltType> Types;

        private int _unknown1;
        private uint _typeHash;

        public Type Type;

        public ArrayType(VltClass.Field field, Type type)
        {
            _unknown1 = field.UnknownUse();
            _typeHash = field.TypeHash;

            Type = type;
            Types = new List<VltType>();
        }

        public override void Read(BinaryReader br)
        {
            Entries = br.ReadInt16();
            MaxEntries = br.ReadInt16();
            _length = br.ReadInt16();

            br.ReadInt16();

            var constructor = Type.GetConstructor(Type.EmptyTypes);

            Debug.Assert(constructor != null, nameof(constructor) + " != null");

            for (var i = 0; i < Entries; ++i)
            {
                if (_unknown1 > 0 && br.BaseStream.Position % _unknown1 != 0L)
                {
                    br.BaseStream.Seek(_unknown1 - br.BaseStream.Position % _unknown1, SeekOrigin.Current);
                }

                var vt = constructor?.Invoke(null) as VltType;

                if (vt is RawType rt)
                {
                    rt.Length = _length;
                }

                Debug.Assert(vt != null, nameof(vt) + " != null");

                vt.Address = (uint)br.BaseStream.Position;
                vt.IsVlt = false;
                vt.TypeHash = _typeHash;
                vt.Info = Info;
                vt.Index = i;
                vt.Read(br);

                Types.Add(vt);
            }
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<VltType> GetEnumerator()
        {
            return Types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(", ", Types.Select(t => t.ToString()));
        }
    }
}