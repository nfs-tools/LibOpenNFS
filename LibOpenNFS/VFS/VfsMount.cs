﻿using System;
using System.Collections.Generic;

namespace LibOpenNFS.VFS
{
    /// <summary>
    /// A mounting point for the VFS. Mounts are essentially directories.
    /// </summary>
    public class VfsMount
    {
        /// <summary>
        /// The mount path.
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// The list of mounted bundles.
        /// </summary>
        public List<VfsBundle> Bundles { get; }
        
        /// <summary>
        /// The list of sub-mounts.
        /// </summary>
        public List<VfsMount> SubMounts { get; }

        /// <summary>
        /// Initialize the mount.
        /// </summary>
        public VfsMount()
        {
            Bundles = new List<VfsBundle>();
            SubMounts = new List<VfsMount>();
        }

        /// <summary>
        /// Mount a bundle.
        /// </summary>
        /// <param name="bundle"></param>
        public void MountBundle(VfsBundle bundle)
        {
            if (Bundles.Exists(b => b.ID == bundle.ID))
            {
                throw new Exception($"Bundle [{bundle.ID}] is already mounted to [{Path}].");
            }
            
            Bundles.Add(bundle);
        }
    }
}