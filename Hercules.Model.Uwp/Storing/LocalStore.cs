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
using Microsoft.HockeyApp;

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
                try
                {
                    StorageFolder folder = await GetStorageFolderAsync();

                    return (await folder.GetFilesAsync()).ToList();
                }
                catch (Exception e)
                {
                    HockeyClient.Current.TrackException(e, GetExceptionProperies("GetFiles"));
                    throw;
                }
            });
        }

        public static Task<StorageFile> CreateFileQueuedAsync(string name)
        {
            Guard.ValidFileName(name, nameof(name));

            return FileQueue.EnqueueAsync(async () =>
            {
                try
                {
                    StorageFolder folder = await GetStorageFolderAsync();

                    return await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);

                }
                catch (Exception e)
                {
                    HockeyClient.Current.TrackException(e, GetExceptionProperies("CreateNew", name));
                    throw;
                }
            });
        }

        public static Task<StorageFile> CreateOrOpenFileQueuedAsync(string name)
        {
            Guard.ValidFileName(name, nameof(name));

            return FileQueue.EnqueueAsync(async () =>
            {
                try
                {
                    StorageFolder folder = await GetStorageFolderAsync();

                    return await folder.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
                }
                catch (Exception e)
                {
                    HockeyClient.Current.TrackException(e, GetExceptionProperies("CreateOrOpen", name));
                    throw;
                }
            });
        }

        private static IDictionary<string, string> GetExceptionProperies(string action, string name = null)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>
            {
                { "FileAction", action }
            };

            if (name != null)
            {
                properties.Add("FileName", name);
            }

            return properties;
        }
    }
}
