// ==========================================================================
// StorageExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace GP.Utils.IO
{
    internal static class StorageExtensions
    {
        public static IAsyncOperation<IReadOnlyList<StorageFile>> GetFilesAsync(this IStorageFolderQueryOperations folder, FileExtension extension)
        {
            QueryOptions options = new QueryOptions(CommonFileQuery.DefaultQuery,
                new[]
                {
                    extension.Extension
                });

            StorageFileQueryResult results = folder.CreateFileQueryWithOptions(options);

            return results.GetFilesAsync();
        }

        public static async Task<StorageFolder> GetOrCreateFolderAsync(this StorageFolder localFolder, string name)
        {
            StorageFolder folder;
            try
            {
                folder = await localFolder.GetFolderAsync(name);
            }
            catch (FileNotFoundException)
            {
                folder = await localFolder.CreateFolderAsync(name);
            }

            return folder;
        }

        public static async Task<StorageFile> GetOrCreateFileAsync(this StorageFolder localFolder, string name)
        {
            StorageFile folder;
            try
            {
                folder = await localFolder.GetFileAsync(name);
            }
            catch (FileNotFoundException)
            {
                folder = await localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            }

            return folder;
        }

        public static async Task<bool> TryDeleteIfExistsAsync(this StorageFolder localFolder, string name)
        {
            try
            {
                StorageFile file = await localFolder.GetFileAsync(name);

                if (file != null)
                {
                    await file.DeleteAsync();

                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            return false;
        }
    }
}
