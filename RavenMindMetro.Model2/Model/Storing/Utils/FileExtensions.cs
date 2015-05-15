// ==========================================================================
// FileExtensions.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace RavenMind.Model.Storing.Utils
{
    internal static class FileExtensions
    {
        public static byte[] SerializeXml(this object content, XmlSerializer xmlSerializer)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, content);

                return memoryStream.ToArray();
            }
        }

        public static async Task<StorageFolder> GetOrCreateFolderAsync(this StorageFolder localFolder, string name)
        { 
            StorageFolder folder = null;
            try
            {
                folder = await localFolder.GetFolderAsync(name);
            }
            catch (FileNotFoundException)
            {
            }

            if (folder == null)
            {
                folder = await localFolder.CreateFolderAsync(name);
            }

            return folder;
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
            }

            return false;
        }
    }
}
