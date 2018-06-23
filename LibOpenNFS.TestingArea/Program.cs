using System;
using System.IO;
using LibOpenNFS.Utils;
using LibOpenNFS.VFS;

namespace LibOpenNFS.TestingArea
{
    internal static class Program
    {
        public static unsafe void Main(string[] args)
        {
            // VFS testing
            {
                var vfsManager = VfsManager.Instance;

                var bundle1 = VfsManager.CreateBundle("TestBundle");
                var bundle2 = VfsManager.CreateBundle("TestBundle2");
                var bundle3 = VfsManager.CreateBundle("TestBundle3");

                bundle1.MountResource(VfsManager.CreateTexturePackResource());
                bundle1.MountResource(VfsManager.CreateTexturePackResource());
                bundle3.MountResource(VfsManager.CreateTexturePackResource());

                vfsManager.MountBundle("/", bundle1);
                vfsManager.MountBundle("/Global", bundle1);
                vfsManager.MountBundle("/Global", bundle2);
                vfsManager.MountBundle("/Global/Test", bundle3);
                
                vfsManager.UnmountBundle($"/Global/Test/{bundle3.ID}");
            }

            if (args.Length > 0)
            {
                // GameManager testing
                var gameManager = GameManager.Instance;

                gameManager.LoadGame(args[0]);

                if (args.Length > 1)
                {
                    gameManager.GetLoader().LoadFile(args[1]);
                    //Console.ReadKey();
                }
                else
                {
                    gameManager.GetLoader().LoadMapStream();
                    //Console.ReadKey();
                    //gameManager.GetLoader().LoadDatabase();
                }
            }
        }
    }
}