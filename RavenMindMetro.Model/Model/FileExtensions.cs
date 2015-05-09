// ==========================================================================
// FileExtensions.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace RavenMind.Model
{
    internal static class FileExtensions
    {
        public static byte[] Deserialize(this object content, XmlSerializer xmlSerializer)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, content);

                return memoryStream.ToArray();
            }
        }

        public static void WriteData(this StorageFolder localFolder, string name, byte[] contents)
        {
            StorageFile file = localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting).AsTask().Result;

            FileIO.WriteBytesAsync(file, contents).AsTask().Wait();
        }

        public static void WriteText(this StorageFolder localFolder, string name, string contents)
        {
            StorageFile file = localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting).AsTask().Result;

            FileIO.WriteTextAsync(file, contents).AsTask().Wait();
        }

        public static StorageFolder CreateFolder(this StorageFolder localFolder, string name)
        {
            return localFolder.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists).AsTask().Result;
        }

        public static string ReadText(this StorageFile file)
        {
            return FileIO.ReadTextAsync(file).AsTask().Result;
        }

        public static IRandomAccessStream Open(this StorageFile file)
        {
            return file.OpenAsync(FileAccessMode.Read).AsTask().Result;
        }

        public static BasicProperties GetProperties(this StorageFile file)
        {
            return file.GetBasicPropertiesAsync().AsTask().Result;
        }

        public static void Delete(this StorageFolder localFolder)
        {
            localFolder.DeleteAsync().AsTask().Wait();
        }

        public static StorageFile GetFile(this StorageFolder localFolder, string name)
        {
            return localFolder.GetFileAsync(name).AsTask().Result;
        }

        public static IReadOnlyList<StorageFile> GetFiles(this StorageFolder localFolder)
        {
            return localFolder.GetFilesAsync().AsTask().Result;
        }

        public static bool TryDeleteIfExists(this StorageFolder localFolder, string name)
        {
            try
            {
                StorageFile file = localFolder.GetFileAsync(name).AsTask().Result;

                file.DeleteAsync().AsTask().Wait();

                return true;
            }
            catch (AggregateException e)
            {
                if (e.InnerException is FileNotFoundException)
                {
                    return false;
                }
                else
                {
                    throw e.InnerException;
                }
            }
        }
    }
}
