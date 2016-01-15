// ==========================================================================
// StorageFolder.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace GP.Utils.IO
{
    internal sealed class Folder : FolderBase
    {
        private readonly string path;

        public Folder(string path)
        {
            this.path = path;
        }

        public override async Task<StorageFolder> GetFolderAsync()
        {
            return await StorageFolder.GetFolderFromPathAsync(path);
        }
    }
}
