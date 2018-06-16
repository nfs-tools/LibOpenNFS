namespace LibOpenNFS.Games.MW.Database.Tree
{
    public class VltClassItem : VltTreeItem
    {
        public readonly VltClass Class;

        public VltClassItem(VltClass @class)
        {
            Class = @class;
        }

        public override string ToString() => HashManager.HashToValue(Class.Hash);

        protected bool Equals(VltClassItem other)
        {
            return Class.Hash == other.Class.Hash;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VltClassItem) obj);
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}