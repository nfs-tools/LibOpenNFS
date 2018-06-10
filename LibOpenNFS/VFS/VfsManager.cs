using System;
using System.Collections.Generic;
using System.Linq;
using LibOpenNFS.VFS.Resources;

namespace LibOpenNFS.VFS
{
    /// <summary>
    /// Manages the virtual file system (VFS).
    /// The VFS manages bundles and resources as if they were directories and files.
    /// </summary>
    public class VfsManager
    {
        private static Lazy<VfsManager> Lazy => new Lazy<VfsManager>(() => new VfsManager());

        public static VfsManager Instance => Lazy.Value;

        private readonly Dictionary<string, VfsMount> _mounts;

        /// <summary>
        /// Initialize the VFS manager.
        /// </summary>
        /// <remarks>private because this class is a singleton.</remarks>
        private VfsManager()
        {
            _mounts = new Dictionary<string, VfsMount>();
        }

        /// <summary>
        /// Create an empty bundle with a random ID and the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static VfsBundle CreateBundle(string name)
        {
            return new VfsBundle(Guid.NewGuid(), name);
        }

        /// <summary>
        /// Create a <see cref="TexturePackResource"/> object.
        /// </summary>
        /// <returns>The new resource.</returns>
        public static TexturePackResource CreateTexturePackResource()
        {
            return new TexturePackResource($"Test TPK {Guid.NewGuid().ToString()}");
        }

        /// <summary>
        /// Mounts a bundle to the virtual filesystem.
        /// </summary>
        /// <param name="location">The mount location. Example: /bundles</param>
        /// <param name="bundle"></param>
        public void MountBundle(string location, VfsBundle bundle)
        {
            if (location.Length == 0)
            {
                throw new ArgumentException("Location cannot be empty", nameof(location));
            }

            var locationParts = location.Split('/');
            var root = locationParts[0];

            if (location == "/")
            {
                root = "/";
                locationParts = new string[0];
            } else if (root.Length == 0)
            {
                root = "/";
            }

            if (_mounts.ContainsKey(root))
            {
                var rootMount = _mounts[root];

                // Do we need submounts?
                if (locationParts.Length > 1)
                {
                    var partQueue = new Queue<string>(locationParts.Skip(1));
                    var builtPath = $"{root}";

                    VfsMount lastMount = null;

                    while (partQueue.Count > 0)
                    {
                        var part = partQueue.Dequeue();

                        if (lastMount == null)
                        {
                            if (!rootMount.SubMounts.Exists(m => m.Path == $"/{part}"))
                            {
                                rootMount.SubMounts.Add(new VfsMount
                                {
                                    Path = $"/{part}"
                                });
                            }

                            builtPath += $"{part}";
                            lastMount = rootMount.SubMounts.Find(m => m.Path == $"/{part}");
                        }
                        else
                        {
                            builtPath += $"/{part}";

                            if (!lastMount.SubMounts.Exists(m => m.Path == builtPath))
                            {
                                lastMount.SubMounts.Add(new VfsMount
                                {
                                    Path = builtPath
                                });
                            }

                            lastMount = lastMount.SubMounts.Find(m => m.Path == builtPath);
                        }
                    }

                    lastMount?.MountBundle(bundle);
                }
                else
                {
                    rootMount?.MountBundle(bundle);
                }
            }
            else
            {
                _mounts.Add(root, new VfsMount
                {
                    Path = root
                });
                
                MountBundle(location, bundle);
            }
        }

        /// <summary>
        /// Unmount a bundle from the virtual filesystem.
        /// </summary>
        /// <param name="location">The full mount location. Example: /bundles/BUNDLEGUID</param>
        public void UnmountBundle(string location)
        {
            var parts = location.Split('/').Skip(1).ToList();
            var mountParts = parts.Take(parts.Count - 1);
            var mountPath = "/" + string.Join("/", mountParts);

            Console.WriteLine("ok");
        }
        
        /// <summary>
        /// Attempt to find a resource by the given ID.
        /// </summary>
        /// <param name="id">The ID of the resource to search for.</param>
        /// <param name="resource"></param>
        /// <typeparam name="TR"></typeparam>
        /// <returns><code>true</code> on success, <code>false</code> on failure.</returns>
        public bool FindResource<TR>(Guid id, out TR resource) where TR : VfsResource
        {
            var result = ScanMountsForResource<TR>(id, _mounts.Values);

            resource = result;
            return result != null;
        }

        /// <summary>
        /// Attempt to find a bundle by the given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bundle"></param>
        /// <returns><code>true</code> on success, <code>false</code> on failure.</returns>
        public bool FindBundle(Guid id, out VfsBundle bundle)
        {
            var result = ScanMountsForBundle(id, _mounts.Values);

            bundle = result;

            return result != null;
        }

        /// <summary>
        /// Internal function to scan VFS mounts for a resource with the given ID.
        /// This supports nested mounts.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mounts"></param>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        private TR ScanMountsForResource<TR>(Guid id, IEnumerable<VfsMount> mounts) where TR : VfsResource
        {
            foreach (var mount in mounts)
            {
                if (mount.Bundles.Count > 0)
                {
                    foreach (var bundle in mount.Bundles)
                    {
                        if (bundle.Resources.Any(r => r.ID == id))
                        {
                            return bundle.Resources.Find(r => r.ID == id) as TR;
                        }
                    }
                }
                else if (mount.SubMounts.Count > 0)
                {
                    return ScanMountsForResource<TR>(id, mount.SubMounts);
                }
            }

            return null;
        }

        /// <summary>
        /// Internal function to scan VFS mounts for a bundle with the given ID.
        /// This supports nested mounts.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mounts"></param>
        /// <returns></returns>
        private VfsBundle ScanMountsForBundle(Guid id, IEnumerable<VfsMount> mounts)
        {
            foreach (var mount in mounts)
            {
                if (mount.Bundles.Exists(b => b.ID == id))
                {
                    return mount.Bundles.Find(b => b.ID == id);
                }

                if (mount.SubMounts.Count > 0)
                {
                    return ScanMountsForBundle(id, mount.SubMounts);
                }
            }

            return null;
        }
    }
}