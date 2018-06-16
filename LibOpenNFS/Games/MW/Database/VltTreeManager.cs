using LibOpenNFS.Games.MW.Database.Tree;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    public class VltTreeManager
    {
        private static VltTreeManager _instance;

        private static readonly object InstanceLock = new object();

        public static VltTreeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new VltTreeManager();
                        }
                    }
                }

                return _instance;
            }
        }

        public readonly Tree<VltTreeItem> Tree;

        private VltTreeManager()
        {
            Tree = new Tree<VltTreeItem>();
        }
    }
}