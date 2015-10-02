// ==========================================================================
// FileExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Hercules.Model.Storing.Utils
{
    internal static class FileExtensions
    {
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

        public static async Task TryWriteDataAsync(this StorageFolder localFolder, string name, IRandomAccessStream contents)
        {
            if (contents != null)
            {
                StorageFile file = await localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);

                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    await contents.AsStream().CopyToAsync(fileStream);
                }
            }
        }

        public static async Task WriteDataAsync(this StorageFolder localFolder, string name, MemoryStream contents)
        {
            StorageFile file = await localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);

            using (Stream fileStream = await file.OpenStreamForWriteAsync())
            {
                contents.WriteTo(fileStream);
            }
        }

        public static async Task WriteDataAsync(this StorageFolder localFolder, string name, byte[] contents)
        {
            StorageFile file = await localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteBytesAsync(file, contents);
        }

        public static async Task WriteTextAsync(this StorageFolder localFolder, string name, string contents)
        {
            StorageFile file = await localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(file, contents);
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
