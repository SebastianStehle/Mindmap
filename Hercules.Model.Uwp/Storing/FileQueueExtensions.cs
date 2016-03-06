// ==========================================================================
// FileQueueExtensions.cs
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
using GP.Utils;
using Hercules.Model.Storing.Json;

namespace Hercules.Model.Storing
{
    public static class FileQueueExtensions
    {
        public static Task DeleteQueuedAsync(this StorageFile file)
        {
            return FileQueue.EnqueueAsync(async () =>
            {
                await file.DeleteAsync(StorageDeleteOption.Default);
            });
        }

        public static Task RenameQueuedAsync(this StorageFile file, string name, string extension)
        {
            Guard.ValidFileName(name, nameof(name));

            return FileQueue.EnqueueAsync(async () =>
            {
                await file.RenameAsync(name + extension, NameCollisionOption.GenerateUniqueName);
            });
        }

        public static Task<Document> OpenDocumentQueuedAsync(this StorageFile file)
        {
            return FileQueue.EnqueueAsync(async () =>
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    return stream.Size > 0 ? JsonDocumentSerializer.Deserialize(stream.AsStreamForRead()) : Document.CreateNew(file.DisplayName);
                }
            });
        }

        public static async Task SaveDocumentQueuedAsync(this StorageFile file, Document document)
        {
            JsonHistory history = new JsonHistory(document);

            await FileQueue.EnqueueAsync(async () =>
            {
                using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
                {
                    JsonDocumentSerializer.Serialize(history, transaction.Stream.AsStreamForWrite());

                    await transaction.CommitAsync();
                }
            });
        }
    }
}
