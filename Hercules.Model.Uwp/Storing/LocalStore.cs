// ==========================================================================
// LocalStore.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using GP.Utils;

namespace Hercules.Model.Storing
{
    public static class LocalStore
    {
        private const string FolderName = "Mindapp";
        private static StorageFolder mindappsFolder;

        private static async Task<StorageFolder> GetStorageFolderAsync()
        {
            if (mindappsFolder != null)
            {
                return mindappsFolder;
            }

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            try
            {
                mindappsFolder = await localFolder.CreateFolderAsync(FolderName);
            }
            catch
            {
                mindappsFolder = await localFolder.GetFolderAsync(FolderName);
            }

            return mindappsFolder;
        }

        public static Task<List<StorageFile>> GetFilesQueuedAsync()
        {
            return FileQueue.EnqueueAsync(async () =>
            {
                StorageFolder folder = await GetStorageFolderAsync();

                return (await folder.GetFilesAsync()).ToList();
            });
        }

        public static Task<StorageFile> CreateFileQueuedAsync(string name)
        {
            Guard.ValidFileName(name, nameof(name));

            return FileQueue.EnqueueAsync(async () =>
            {
                StorageFolder folder = await GetStorageFolderAsync();

                return await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
            });
        }
    }
}
