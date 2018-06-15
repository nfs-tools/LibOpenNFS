using System.Collections;
using LibOpenNFS.Games.MW.Database.Table;

namespace LibOpenNFS.Games.MW.Database
{
    public class VltInfo : IEnumerable
    {
        public VltBlockContainer BlockContainer;
        public VltRowRecord RowRecord;
        public VltType[] Types;
        public bool[] TypesPresent;
        public VltClass Class;

        public VltInfo(int numTypes)
        {
            Types = new VltType[numTypes];
            TypesPresent = new bool[numTypes];
        }

        public void Set(int index, VltType type)
        {
            TypesPresent[index] = true;
            Types[index] = type;
        }

        public IEnumerator GetEnumerator() => Types.GetEnumerator();
    }
}