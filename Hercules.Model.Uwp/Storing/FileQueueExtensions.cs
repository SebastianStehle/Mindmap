// ==========================================================================
// FileQueueExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using GP.Utils;
using Hercules.Model.Storing.Json;
using Microsoft.HockeyApp;

namespace Hercules.Model.Storing
{
    public static class FileQueueExtensions
    {
        public static Task DeleteQueuedAsync(this StorageFile file)
        {
            return FileQueue.EnqueueAsync(async () =>
            {
                try
                {
                    await file.DeleteAsync(StorageDeleteOption.Default);
                }
                catch (Exception e)
                {
                    HockeyClient.Current.TrackException(e, GetExceptionProperies(file, "Delete"));
                    throw;
                }
            });
        }

        public static Task RenameQueuedAsync(this StorageFile file, string name, string extension)
        {
            Guard.ValidFileName(name, nameof(name));

            return FileQueue.EnqueueAsync(async () =>
            {
                try
                {
                    await file.RenameAsync(name + extension, NameCollisionOption.GenerateUniqueName);
                }
                catch (Exception e)
                {
                    HockeyClient.Current.TrackException(e, GetExceptionProperies(file, "Rename"));
                    throw;
                }
            });
        }

        public static Task<Document> OpenDocumentQueuedAsync(this StorageFile file)
        {
            return FileQueue.EnqueueAsync(async () =>
            {
                try
                {
                    using (IRandomAccessStream stream = await file.OpenReadAsync())
                    {
                        return stream.Size > 0 ? JsonDocumentSerializer.Deserialize(stream.AsStreamForRead()) : new Document(Guid.NewGuid(), file.DisplayName);
                    }
                }
                catch (Exception e)
                {
                    HockeyClient.Current.TrackException(e, GetExceptionProperies(file, "Open"));
                    throw;
                }
            });
        }

        public static async Task SaveDocumentQueuedAsync(this StorageFile file, Document document)
        {
            JsonHistory history = new JsonHistory(document);

            await FileQueue.EnqueueAsync(async () =>
            {
                try
                {
                    using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
                    {
                        JsonDocumentSerializer.Serialize(history, transaction.Stream.AsStreamForWrite());

                        await transaction.CommitAsync();
                    }
                }
                catch (Exception e)
                {
                    HockeyClient.Current.TrackException(e, GetExceptionProperies(file, "Open"));
                    throw;
                }
            });
        }

        private static IDictionary<string, string> GetExceptionProperies(IStorageItem file, string action)
        {
            return new Dictionary<string, string>
            {
                { "FileName", !string.IsNullOrWhiteSpace(file.Path) ? file.Path : file.Name },
                { "FileAction", action }
            };
        }
    }
}
