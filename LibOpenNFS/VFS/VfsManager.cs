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
//        private static Lazy<VfsManager> Lazy => new Lazy<VfsManager>(() => new VfsManager());
//
//        public static VfsManager Instance => Lazy.Value;

        private static VfsManager _instance;

        private static readonly object InstanceLock = new object();

        public static VfsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new VfsManager();
                        }
                    }
                }

                return _instance;
            }
        }

        private Dictionary<string, VfsMount> _mounts;

        /// <summary>
        /// Initialize the VFS manager.
        /// </summary>
        /// <remarks>private because this class is a singleton.</remarks>
        private VfsManager()
        {
            Reset();
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
        /// Reset the VFS. This clears all mount points.
        /// </summary>
        public void Reset()
        {
            _mounts = new Dictionary<string, VfsMount>
            {
                {
                    "/", new VfsMount
                    {
                        Path = "/"
                    }
                }
            };
        }

        /// <summary>
        /// Mounts a bundle to the VFS.
        /// </summary>
        /// <param name="location">The mount location. Example: /bundles</param>
        /// <param name="bundle"></param>
        public void MountBundle(string location, VfsBundle bundle)
        {
            if (location.Length == 0)
            {
                throw new ArgumentException("Location cannot be empty", nameof(location));
            }

            // Make sure the root mount exists
            if (!FindMount("/", out var rootMount))
            {
                throw new Exception("Failed to access root mount.");
            }

            var parts = location.Split('/').Skip(1).ToList();

            if (parts.Count == 1)
            {
                if (string.IsNullOrWhiteSpace(parts[0]))
                {
                    // Trying to mount to root
                    rootMount.MountBundle(bundle);
                }
                else
                {
                    if (rootMount.SubMounts.Exists(m => m.Path == $"/{parts[0]}"))
                    {
                        rootMount.SubMounts.Find(m => m.Path == $"/{parts[0]}")
                            .MountBundle(bundle);
                    }
                    else
                    {
                        // Create a sub-mount under root
                        rootMount.SubMounts.Add(new VfsMount
                        {
                            Path = $"/{parts[0]}",
                            Bundles = {bundle}
                        });
                    }
                }
            }
            else
            {
                var lastMount = rootMount;
                var partQueue = new Queue<string>(parts);
                var builtPath = "/";

                while (partQueue.Count > 0)
                {
                    var part = partQueue.Dequeue();

                    if (lastMount.Path == rootMount.Path)
                    {
                        builtPath += part;

                        if (!rootMount.SubMounts.Exists(m => m.Path == builtPath))
                        {
                            rootMount.SubMounts.Add(new VfsMount
                            {
                                Path = builtPath
                            });
                        }

                        lastMount = rootMount.SubMounts.Find(m => m.Path == builtPath);
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

                lastMount.MountBundle(bundle);
            }

            _mounts["/"] = rootMount;
        }

        /// <summary>
        /// Unmount a bundle from the VFS.
        /// </summary>
        /// <param name="location">The full mount location. Example: /bundles/BUNDLEGUID</param>
        public void UnmountBundle(string location)
        {
            var parts = location.Split('/').Skip(1).ToList();
            var mountParts = parts.Take(parts.Count - 1);
            var mountPath = "/" + string.Join("/", mountParts);

            if (!FindMount(mountPath, out var mount))
            {
                throw new Exception("Cannot find mount for bundle");
            }

            mount.UnmountBundle(Guid.Parse(parts.Last()));
        }

        /// <summary>
        /// Attempt to find a VFS mount point.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mount"></param>
        /// <returns></returns>
        public bool FindMount(string path, out VfsMount mount)
        {
            var parts = path.Split('/').ToList();
            var root = parts[0];

            if (path == "/")
            {
                root = "/";
                parts = new List<string>();
            }
            else if (root.Length == 0)
            {
                root = "/";
            }

            // Check to see if we're accessing the root.
            if (path == "/")
            {
                if (!_mounts.ContainsKey(root))
                {
                    // This shouldn't ever happen. Ever. Only if something really stupid is going on.
                    throw new Exception($"Cannot access root mount.");
                }

                mount = _mounts[root];

                return true;
            }

            if (_mounts.ContainsKey(root))
            {
                var rootMount = _mounts[root];

                if (parts.Count > 1)
                {
                    var partQueue = new Queue<string>(parts.Skip(1));
                    var builtPath = $"{root}";

                    VfsMount lastMount = null;

                    while (partQueue.Count > 0)
                    {
                        var part = partQueue.Dequeue();

                        if (lastMount == null)
                        {
                            if (!rootMount.SubMounts.Exists(m => m.Path == $"/{part}"))
                            {
                                throw new Exception($"Cannot find mount @ [/{part}]");
                            }

                            builtPath += $"{part}";
                            lastMount = rootMount.SubMounts.Find(m => m.Path == $"/{part}");
                        }
                        else
                        {
                            builtPath += $"/{part}";

                            if (!lastMount.SubMounts.Exists(m => m.Path == builtPath))
                            {
                                throw new Exception($"Cannot find mount @ [{builtPath}]");
                            }

                            lastMount = lastMount.SubMounts.Find(m => m.Path == builtPath);
                        }
                    }

                    mount = lastMount;
                    return true;
                }
            }

            mount = null;

            return false;
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
        /// Attempt to find a bundle by the given ID.
        /// </summary>
        /// <param name="id"></param>
        public VfsBundle FindBundle(Guid id)
        {
            var result = ScanMountsForBundle(id, _mounts.Values);

            return result;
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
                    var result = ScanBundlesForResource<TR>(id, mount.Bundles);

                    if (result != null)
                    {
                        return result;
                    }
                }

                if (mount.SubMounts.Count > 0)
                {
                    var result = ScanMountsForResource<TR>(id, mount.SubMounts);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private TR ScanBundlesForResource<TR>(Guid id, IEnumerable<VfsBundle> bundles) where TR : VfsResource
        {
            foreach (var bundle in bundles)
            {
                if (bundle.Resources.Exists(r => r.ID == id))
                {
                    return bundle.Resources.Find(r => r.ID == id) as TR;
                }

                if (bundle.Bundles.Count > 0)
                {
                    return ScanBundlesForResource<TR>(id, bundle.Bundles);
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
                    var result = ScanMountsForBundle(id, mount.SubMounts);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }
}