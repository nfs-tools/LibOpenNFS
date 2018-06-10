using System;

namespace LibOpenNFS.VFS
{
    /// <summary>
    /// A resource within a VFS bundle.
    /// This is abstract because there are multiple types of resources.
    /// </summary>
    public abstract class VfsResource
    {
        /// <summary>
        /// The resource's unique identifier.
        /// </summary>
        public Guid ID { get; }
        
        /// <summary>
        /// The name of the resource.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initialize the resource.
        /// This just generates a GUID.
        /// </summary>
        protected VfsResource()
        {
            ID = Guid.NewGuid();
        }
    }
}