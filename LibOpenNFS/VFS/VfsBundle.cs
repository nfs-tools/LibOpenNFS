using System;
using System.Collections.Generic;

namespace LibOpenNFS.VFS
{
    /// <summary>
    /// A bundle within the VFS. Bundles contain resources.
    /// </summary>
    public class VfsBundle
    {
        /// <summary>
        /// The list of resources within the bundle.
        /// </summary>
        public List<VfsResource> Resources { get; }
        
        /// <summary>
        /// The list of sub-bundles.
        /// </summary>
        public List<VfsBundle> Bundles { get; }

        /// <summary>
        /// The name of the bundle.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The bundle's unique identifier.
        /// </summary>
        public Guid ID { get; }
        
        /// <summary>
        /// Initialize the bundle.
        /// </summary>
        /// <param name="id">The bundle's unique identifier.</param>
        /// <param name="name">The name of the bundle.</param>
        public VfsBundle(Guid id, string name)
        {
            Name = name;
            ID = id;
            Resources = new List<VfsResource>();
            Bundles = new List<VfsBundle>();
        }
        
        /// <summary>
        /// Mount a bundle to the bundle. Yes, seriously.
        /// </summary>
        /// <param name="bundle"></param>
        public VfsBundle MountBundle(VfsBundle bundle)
        {
            if (Bundles.Exists(b => b.ID == bundle.ID))
            {
                throw new Exception($"Bundle [{bundle.ID}] is already mounted to bundle {Name}.");
            }
            
            Bundles.Add(bundle);

            return Bundles.Find(b => b.ID == bundle.ID);
        }

        /// <summary>
        /// Mount a resource to the bundle.
        /// </summary>
        /// <param name="resource"></param>
        /// <typeparam name="TR"></typeparam>
        public TR MountResource<TR>(TR resource) where TR : VfsResource
        {
            if (Resources.Exists(r => r.ID == resource.ID))
            {
                throw new Exception($"Resource [{resource.ID}] is already mounted to bundle {Name}.");
            }
            
            Resources.Add(resource);

            return Resources.Find(r => r.ID == resource.ID) as TR;
        }
    }
}