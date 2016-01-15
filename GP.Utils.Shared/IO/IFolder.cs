// ==========================================================================
// IFolder.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GP.Utils.IO
{
    /// <summary>
    /// Represents a user folder.
    /// </summary>
    public interface IFolder
    {
        /// <summary>
        /// Gets all files with the specified extension.
        /// </summary>
        /// <param name="extension">The requested file extension. Cannot be null.</param>
        /// <returns>
        /// The list of files.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/> is null.</exception>
        Task<List<IFile>> GetFilesAsync(FileExtension extension);

        /// <summary>
        /// Creates a new file with the specified name.
        /// </summary>
        /// <param name="desiredName">The name of the file.</param>
        /// <param name="extension">The requested file extension. Cannot be null.</param>
        /// <returns>
        /// The created file.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="desiredName"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="desiredName"/> is not a valid file name.</exception>
        Task<IFile> CreateFileAsync(string desiredName, FileExtension extension);

        /// <summary>
        /// Copies all files with an extension from the specified folder.
        /// </summary>
        /// <param name="source">The source folder. Cannot be null.</param>
        /// <param name="extension">The requested file extension. Cannot be null.</param>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/> is null.</exception>
        Task CopyFilesAsync(IFolder source, FileExtension extension);

        /// <summary>
        /// Opens specified file asynchronously and returns the content.
        /// </summary>
        /// <param name="file">The file to read. Cannot be null.</param>
        /// <returns>
        /// The content of the file.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
        Task<byte[]> OpenAsync(IFile file);

        /// <summary>
        /// Writes the contents to the specified file asynchronously.
        /// </summary>
        /// <param name="file">The file to write. Cannot be null.</param>
        /// <param name="contents">The contents of the file. Cannot be null.</param>
        /// <returns>
        /// The task for synchronization.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
        Task SaveAsync(IFile file, byte[] contents);

        /// <summary>
        /// Renames the specified file asynchronously.
        /// </summary>
        /// <param name="file">The file to rename. Cannot be null.</param>
        /// <param name="desiredName">The desired name of the file.</param>
        /// <returns>
        /// The created file.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="desiredName"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="desiredName"/> is not a valid file name.</exception>
        Task RenameAsync(IFile file, string desiredName);

        /// <summary>
        /// Deletes the specified file asynchronously.
        /// </summary>
        /// <param name="file">The file to delete. Cannot be null.</param>
        /// <returns>
        /// True, if the file has been deleted or false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
        Task<bool> DeleteAsync(IFile file);
    }
}