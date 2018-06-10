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
        /// The name of the bundle.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The bundle's unique identifier.
        /// </summary>
        public Guid ID { get; }
        
        /// <summary>
        /// Instantiate a new bundle.
        /// </summary>
        /// <param name="id">The bundle's unique identifier.</param>
        /// <param name="name">The name of the bundle.</param>
        public VfsBundle(Guid id, string name)
        {
            Name = name;
            ID = id;
            Resources = new List<VfsResource>();
        }

        /// <summary>
        /// Mount a resource to the bundle.
        /// </summary>
        /// <param name="resource"></param>
        /// <typeparam name="TR"></typeparam>
        public TR MountResource<TR>(TR resource) where TR : VfsResource
        {
            Resources.Add(resource);

            return Resources.Find(r => r == resource) as TR;
        }
    }
}