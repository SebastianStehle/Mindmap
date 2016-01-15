// ==========================================================================
// IFileSystem.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace GP.Utils.IO
{
    /// <summary>
    /// Wrapps access to a user specific file storage.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Gets a folder by a path.
        /// </summary>
        /// <param name="path">The path of the folder.</param>
        /// <returns>
        /// The user folder.
        /// </returns>
        IFolder GetCustomFolder(string path);

        /// <summary>
        /// Gets a folder with a name in the default storage.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        /// <returns>
        /// The user folder.
        /// </returns>
        IFolder GetLocalFolder(string name);
    }
}
