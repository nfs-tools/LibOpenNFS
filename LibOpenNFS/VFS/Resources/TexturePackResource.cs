namespace LibOpenNFS.VFS.Resources
{
    /// <inheritdoc />
    /// <summary>
    /// A texture pack VFS resource.
    /// </summary>
    public class TexturePackResource : VfsResource
    {
        /// <summary>
        /// The name of the texture pack.
        /// </summary>
        public string PackName { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Initialize the resource.
        /// </summary>
        /// <param name="packName"></param>
        public TexturePackResource(string packName)
        {
            PackName = packName;
        }
    }
}