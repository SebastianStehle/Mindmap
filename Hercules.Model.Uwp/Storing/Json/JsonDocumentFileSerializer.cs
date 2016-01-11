// ==========================================================================
// JsonDocumentSerializer.cs
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

namespace Hercules.Model.Storing.Json
{
    public static class JsonDocumentFileSerializer
    {
        public static async Task SerializeToFileAsync(StorageFile file, Document document)
        {
            Guard.NotNull(file, nameof(file));
            Guard.NotNull(document, nameof(document));

            JsonHistory history = new JsonHistory(document);

            using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync(StorageOpenOptions.None))
            {
                Stream fileStream = transaction.Stream.AsStreamForWrite();

                JsonDocumentSerializer.SerializeToStream(fileStream, history);

                await transaction.CommitAsync();
            }
        }

        public static async Task SerializeToFileAsync(StorageFile file, JsonHistory history)
        {
            Guard.NotNull(file, nameof(file));
            Guard.NotNull(history, nameof(history));

            using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync(StorageOpenOptions.None))
            {
                Stream fileStream = transaction.Stream.AsStreamForWrite();

                JsonDocumentSerializer.SerializeToStream(fileStream, history);

                await transaction.CommitAsync();
            }
        }

        public static async Task<Document> DeserializeFromFileAsync(StorageFile file)
        {
            Guard.NotNull(file, nameof(file));

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                Stream fileStream = stream.AsStreamForRead();

                Document document = JsonDocumentSerializer.DeserializeDocumentFromStream(fileStream);

                return document;
            }
        }
    }
}
