// ==========================================================================
// FileExtension.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;

namespace GP.Windows
{
    /// <summary>
    /// Represents a file extension as a pair of extension and mime type.
    /// </summary>
    public sealed partial class FileExtension : IEquatable<FileExtension>
    {
        private readonly string extension;
        private readonly string mimeType;

        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        public string Extension
        {
            get
            {
                return extension;
            }
        }

        /// <summary>
        /// Gets the mime type.
        /// </summary>
        /// <value>
        /// The mime type.
        /// </value>
        public string MimeType
        {
            get
            {
                return mimeType;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtension"/> class with the extension and the mime type.
        /// </summary>
        /// <param name="extension">The extension. Cannot be null or empty.</param>
        /// <param name="mimeType">Type mime type. Cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="extension"/> is null.
        /// - or -
        /// <paramref name="mimeType"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="extension"/> is empty or contains only whitespaces.
        /// - or -
        /// <paramref name="mimeType"/> is empty or contains only whitespaces.
        /// </exception>
        public FileExtension(string extension, string mimeType)
        {
            Guard.NotNullOrEmpty(extension, "extension");
            Guard.NotNullOrEmpty(mimeType, "mimeType");

            this.extension = extension;
            this.mimeType = mimeType;
        }

        /// <summary>
        /// Determines if the file extension matchs the file extension of the specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// True, if the extension matchs or false otherwise.
        /// </returns>
        public bool MatchsFileName(string fileName)
        {
            bool result = false;

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string fileNameExtension = Path.GetExtension(fileName);

                result = string.Equals(fileNameExtension, extension, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }

        private static FileExtension CreateFileExtension(string extension, string mimeType)
        {
            var fileExtension = new FileExtension(extension, mimeType);

            extensionMappings.Add(extension, fileExtension);

            return fileExtension;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return extension.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(FileExtension other)
        {
            return other != null && string.Equals(other.extension, extension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"></see>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as FileExtension);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return extension;
        }

        /// <summary>
        /// Gets the file extension by the extension.
        /// </summary>
        /// <param name="extension">The extension. Cannot be null.</param>
        /// <returns>
        /// The file extension.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/> is null.</exception>
        public static FileExtension GetFileExtension(string extension)
        {
            Guard.NotNull(extension, nameof(extension));

            return extensionMappings[extension];
        }
    }
}
