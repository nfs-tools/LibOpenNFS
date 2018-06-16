using LibOpenNFS.Games.MW.Database.Table;

namespace LibOpenNFS.Games.MW.Database.Tree
{
    public class VltRowItem : VltTreeItem
    {
        public readonly VltRowRecord RowRecord;

        public VltRowItem(VltRowRecord rowRecord)
        {
            RowRecord = rowRecord;
        }

        public override string ToString() => HashManager.HashToValue(RowRecord.Hash);

        protected bool Equals(VltRowItem other)
        {
            return RowRecord.Hash == other.RowRecord.Hash;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VltRowItem) obj);
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}