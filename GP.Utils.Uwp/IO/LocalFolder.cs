// ==========================================================================
// LocalFolder.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Windows.Storage;

namespace GP.Utils.IO
{
    internal sealed class LocalFolder : FolderBase
    {
        private readonly string name;

        public LocalFolder(string name)
        {
            this.name = name;
        }

        public override async Task<StorageFolder> GetFolderAsync()
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetOrCreateFolderAsync(name);

            return folder;
        }
    }
}
