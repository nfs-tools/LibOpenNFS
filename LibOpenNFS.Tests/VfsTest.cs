using System;
using LibOpenNFS.VFS;
using LibOpenNFS.VFS.Resources;
using NUnit.Framework;

namespace LibOpenNFS.Tests
{
    [TestFixture]
    public class VfsTest
    {
        [Test]
        public void TestMounting()
        {
            var vfsManager = VfsManager.Instance;
            
            var bundle1 = VfsManager.CreateBundle("TestBundle1");
            var testTpk1 = VfsManager.CreateTexturePackResource();
            
            var bundle2 = VfsManager.CreateBundle("TestBundle2");
            var testTpk2 = VfsManager.CreateTexturePackResource();

            var bundle3 = VfsManager.CreateBundle("TestBundle3");

            bundle1.MountResource(testTpk1);
            bundle2.MountResource(testTpk1);
            bundle2.MountResource(testTpk2);

            var bundle2Child = VfsManager.CreateBundle("Bundle2_Child");
            bundle2Child.MountResource(testTpk1);

            bundle2.MountBundle(bundle2Child);
            
            vfsManager.MountBundle("/", bundle1);
            vfsManager.MountBundle("/test", bundle2);
            vfsManager.MountBundle("/test/nested/paths", bundle3);
            
            Assert.True(vfsManager.FindBundle(bundle1.ID, out _));
            Assert.True(vfsManager.FindResource<TexturePackResource>(testTpk1.ID, out _));
            
            Assert.Throws<Exception>(() => vfsManager.MountBundle("/", bundle1));
            
            Assert.True(vfsManager.FindBundle(bundle2.ID, out _));
            Assert.True(vfsManager.FindBundle(bundle3.ID, out _));
            
            vfsManager.UnmountBundle($"/test/nested/paths/{bundle3.ID}");
            
            Assert.False(vfsManager.FindBundle(bundle3.ID, out _));

//            Assert.True(true);
        }
    }
}