using System;
using LibOpenNFS.VFS;

namespace LibOpenNFS.TestingArea
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var vfsManager = VfsManager.Instance;
            
            var bundle1 = VfsManager.CreateBundle("TestBundle");
            var bundle2 = VfsManager.CreateBundle("TestBundle2");
            var bundle3 = VfsManager.CreateBundle("TestBundle3");
            
            var tpk1 = bundle1.MountResource(VfsManager.CreateTexturePackResource());
            var tpk2 = bundle1.MountResource(VfsManager.CreateTexturePackResource());
            var tpk3 = bundle3.MountResource(VfsManager.CreateTexturePackResource());
            
            vfsManager.MountBundle("/Global", bundle1);
            vfsManager.MountBundle("/Global", bundle2);
            vfsManager.MountBundle("/Global/Test/Path", bundle3);
            
            Console.WriteLine("Done!");
        }
    }
}