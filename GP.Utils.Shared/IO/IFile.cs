// ==========================================================================
// IFile.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace GP.Utils.IO
{
    /// <summary>
    /// Defines a user file.
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the date and time when the file has been modified.
        /// </summary>
        DateTimeOffset ModifiedUtc { get; }

        /// <summary>
        /// Gets a value indicating if the file is deleted.
        /// </summary>
        bool IsDeleted { get; }
    }
}