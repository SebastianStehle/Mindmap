// ==========================================================================
// StorageFile.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace GP.Utils.IO
{
    internal class File : IFile
    {
        private readonly FileExtension extension;
        private DateTimeOffset modifiedUtc;
        private string name;
        private bool isDeleted;

        public string Name
        {
            get { return name; }
        }

        public string FullName
        {
            get { return name + extension; }
        }

        public bool IsDeleted
        {
            get { return isDeleted; }
        }

        public DateTimeOffset ModifiedUtc
        {
            get { return modifiedUtc; }
        }

        public FileExtension Extension
        {
            get { return extension; }
        }

        public File(string fileName, FileExtension extension, DateTimeOffset modifiedUtc)
        {
            name = NameWithoutExtension(fileName, extension);

            this.extension = extension;

            this.modifiedUtc = modifiedUtc;
        }

        public File Rename(string fileName)
        {
            name = NameWithoutExtension(fileName, extension);

            return this;
        }

        public File Deleted()
        {
            isDeleted = true;

            return this;
        }

        public File Updated()
        {
            modifiedUtc = DateTimeOffset.Now;

            return this;
        }

        private static string NameWithoutExtension(string name, FileExtension extension)
        {
            if (name.EndsWith(extension.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - extension.ToString().Length);
            }

            return name;
        }
    }
}
