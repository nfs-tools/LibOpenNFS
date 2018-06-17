using System;
using System.Collections.Generic;
using LibOpenNFS.Games.MW.Database.Table;

namespace LibOpenNFS.Games.MW.Database.Tree
{
    public class VltRowItem : VltTreeItem
    {
        public readonly VltRowRecord RowRecord;
        public readonly Dictionary<uint, VltType> Fields;

        public VltRowItem(VltRowRecord rowRecord)
        {
            RowRecord = rowRecord;
            Fields = new Dictionary<uint, VltType>();
        }

        public override string ToString()
        {
            var str = $"{HashManager.HashToValue(RowRecord.Hash)}";

            foreach (var field in Fields)
            {
                str += Environment.NewLine + "\t" + $"{HashManager.HashToValue(field.Key)} ({field.Value.GetType()}) -> {field.Value}";
            }

            return str;
        }

        protected bool Equals(VltRowItem other)
        {
            return RowRecord.Hash == other.RowRecord.Hash;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VltRowItem)obj);
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}