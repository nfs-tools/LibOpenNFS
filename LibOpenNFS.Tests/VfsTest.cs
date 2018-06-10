using System;
using System.Data;
using LibOpenNFS.VFS;
using LibOpenNFS.VFS.Resources;
using NUnit.Framework;

namespace LibOpenNFS.Tests
{
    /// <summary>
    /// Tests for the VFS (Virtual File System) code.
    /// </summary>
    [TestFixture, SetUpFixture]
    public class VfsTest
    {
        private VfsManager _vfsManager;

        [OneTimeSetUp]
        public void SetUp()
        {
            _vfsManager = VfsManager.Instance;
        }
        
        /// <summary>
        /// Tests the bundle mounting functionality by creating bundles and loading resources into them,
        /// then ensuring that the bundles and resources can be found.
        /// </summary>
        [Test]
        public void TestMounting()
        {
            var bundle1 = VfsManager.CreateBundle("TestBundle");
            var bundle2 = VfsManager.CreateBundle("TestBundle2");
            var bundle3 = VfsManager.CreateBundle("TestBundle3");
            
            var tpk1 = bundle1.MountResource(VfsManager.CreateTexturePackResource());
            var tpk2 = bundle1.MountResource(VfsManager.CreateTexturePackResource());
            var tpk3 = bundle3.MountResource(VfsManager.CreateTexturePackResource());
            
            _vfsManager.MountBundle("/Global", bundle1);
            _vfsManager.MountBundle("/Global", bundle2);
            _vfsManager.MountBundle("/Global/Test/Path", bundle3);
            
            Assert.True(_vfsManager.FindBundle(bundle1.ID, out _));
            Assert.True(_vfsManager.FindBundle(bundle2.ID, out _));
            Assert.True(_vfsManager.FindBundle(bundle3.ID, out _));
            
            Assert.True(_vfsManager.FindResource<TexturePackResource>(tpk1.ID, out _));
            Assert.True(_vfsManager.FindResource<TexturePackResource>(tpk2.ID, out _));
            Assert.True(_vfsManager.FindResource<TexturePackResource>(tpk3.ID, out _));
        }
    }
}