using System.Collections;
using System.Collections.Generic;
using System.IO;
using LibOpenNFS.Games.MW.Database.Blocks;
using LibOpenNFS.Games.MW.Database.Table;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    public class VltClassManager : IEnumerable<VltClass>
    {
        public class ManagedClass
        {
            public uint Hash { get; set; }

            public string Value { get; set; }

            public int Unknown { get; set; }
        }

        private Dictionary<uint, ManagedClass> _managedClasses;

        public Dictionary<uint, VltClass> Classes { get; }

        private static VltClassManager _instance;

        private static readonly object InstanceLock = new object();

        public static VltClassManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new VltClassManager();
                        }
                    }
                }

                return _instance;
            }
        }

        private VltClassManager()
        {
            Classes = new Dictionary<uint, VltClass>();
        }

        public void Init(VltRootRecord rootRecord, TableEndBlock teb, BinaryReader br)
        {
            var position = teb.InfoDictionary[rootRecord.Position].Address2;

            br.BaseStream.Seek(position, SeekOrigin.Begin);

            _managedClasses = new Dictionary<uint, ManagedClass>(rootRecord.NumEntries);

            for (var i = 0; i < rootRecord.NumEntries; ++i)
            {
                var mc = new ManagedClass
                {
                    Value = BinaryUtil.ReadNullTerminatedString(br),
                    Unknown = rootRecord.Hashes[i]
                };

                mc.Hash = JenkinsHash.getHash32(mc.Value);

                _managedClasses.Add(mc.Hash, mc);
            }
        }

        public void Init(VltClassRecord classRecord, TableEndBlock teb, BinaryReader br)
        {
            var vc = new VltClass();
            vc.Init(classRecord, teb, br);
            Classes.Add(vc.Hash, vc);
        }

        public IEnumerator<VltClass> GetEnumerator()
        {
            return Classes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}