// ==========================================================================
// FileExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using GP.Windows;

namespace Hercules.Model.Storing.Utils
{
    internal static class FileExtensions
    {
        public static Task<List<StorageFile>> GetFilesAsync(this StorageFolder localFolder, FileExtension extension)
        {
            return localFolder.GetFilesAsync(extension.Extension);
        }

        public static string NameWithoutExtension(this StorageFile file, FileExtension extension)
        {
            string name = file.Name;

            if (name.EndsWith(extension.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - extension.ToString().Length);
            }

            return name;
        }

        public static async Task<List<StorageFile>> GetFilesAsync(this StorageFolder localFolder, string extension)
        {
            IEnumerable<StorageFile> files = await localFolder.GetFilesAsync();

            return files.Where(file => file.FileType.Equals(extension, StringComparison.OrdinalIgnoreCase)).ToList();
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
