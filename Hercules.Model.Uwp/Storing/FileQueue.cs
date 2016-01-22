// ==========================================================================
// FileQueue.cs
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
using Windows.System.Threading;
using GP.Utils;
using Hercules.Model.Storing.Json;
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Hercules.Model.Storing
{
    internal static class FileQueue
    {
        private static readonly LimitedConcurrencyLevelTaskScheduler Scheduler = new LimitedConcurrencyLevelTaskScheduler(1, z => ThreadPool.RunAsync(x => z(), WorkItemPriority.Normal).Forget());
        private static readonly TaskFactory TaskFactory;

        static FileQueue()
        {
            TaskFactory = new TaskFactory(Scheduler);
        }

        public static Task EnqueueAsync(Func<Task> action)
        {
            return TaskFactory.StartNew(action).Unwrap();
        }

        public static Task<T> EnqueueAsync<T>(Func<Task<T>> action)
        {
            return TaskFactory.StartNew(action).Unwrap();
        }

        public static Task RenameAsync(StorageFile file, string name, string extension)
        {
            return EnqueueAsync(async () =>
            {
                await file.RenameAsync(name + extension, NameCollisionOption.GenerateUniqueName);
            });
        }

        public static Task<Document> OpenAsync(StorageFile file)
        {
            return EnqueueAsync(async () =>
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    if (stream.Size > 0)
                    {
                        return JsonDocumentSerializer.Deserialize(stream.AsStreamForRead());
                    }

                    return Document.CreateNew(file.DisplayName);
                }
            });
        }

        public static async Task SaveAsync(Document document, StorageFile file)
        {
            JsonHistory history = new JsonHistory(document);

            await EnqueueAsync(async () =>
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
