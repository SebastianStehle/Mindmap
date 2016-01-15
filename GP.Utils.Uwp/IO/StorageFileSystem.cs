// ==========================================================================
// StorageFileSystem.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace GP.Utils.IO
{
    /// <summary>
    /// Implements the <see cref="IFileSystem"/> interface.
    /// </summary>
    public sealed class StorageFileSystem : IFileSystem
    {
        /// <summary>
        /// Gets a folder by a path.
        /// </summary>
        /// <param name="path">The path of the folder.</param>
        /// <returns>
        /// The user folder.
        /// </returns>
        public IFolder GetCustomFolder(string path)
        {
            Guard.NotNullOrEmpty(path, nameof(path));

            return new Folder(path);
        }

        /// <summary>
        /// Gets a folder with a name in the default storage.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        /// <returns>
        /// The user folder.
        /// </returns>
        public IFolder GetLocalFolder(string name)
        {
            Guard.ValidFileName(name, nameof(name));

            return new LocalFolder(name);
        }
    }
}
