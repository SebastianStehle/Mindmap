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
using GP.Windows;
using Hercules.Model.Storing.Utils;
using Newtonsoft.Json;

namespace Hercules.Model.Storing.Json
{
    public static class JsonDocumentSerializer
    {
        private static readonly JsonSerializerSettings HistorySerializerSettings = new JsonSerializerSettings();

        public static FileExtension FileExtension
        {
            get
            {
                return new FileExtension(".mmd", "application/json");
            }
        }

        static JsonDocumentSerializer()
        {
            HistorySerializerSettings.Converters.Add(new PropertiesBagConverter());
        }

        public static void SerializeToStream(Stream stream, Document document)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNull(document, nameof(document));

            JsonHistory history = new JsonHistory(document);

            history.SerializeAsJson(stream, HistorySerializerSettings);
        }

        public static Task SerializeToFileAsync(StorageFile file, Document document)
        {
            Guard.NotNull(file, nameof(file));
            Guard.NotNull(document, nameof(document));

            JsonHistory history = new JsonHistory(document);

            return history.SerializeAsJsonAsync(file, HistorySerializerSettings);
        }

        public static void SerializeToStream(Stream stream, JsonHistory history)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNull(history, nameof(history));

            history.SerializeAsJson(stream, HistorySerializerSettings);
        }

        public static Task SerializeToFileAsync(StorageFile file, JsonHistory history)
        {
            Guard.NotNull(file, nameof(file));
            Guard.NotNull(history, nameof(history));

            return history.SerializeAsJsonAsync(file, HistorySerializerSettings);
        }

        public static Document DeserializeFromStream(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            JsonHistory history = stream.DeserializeAsJson<JsonHistory>();

            return history.ToDocument();
        }

        public static async Task<Document> DeserializeFromFileAsync(StorageFile file)
        {
            Guard.NotNull(file, nameof(file));

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                Stream normalStream = stream.AsStreamForRead();

                JsonHistory history = normalStream.DeserializeAsJson<JsonHistory>(HistorySerializerSettings);

                Document document = history.ToDocument();

                return document;
            }
        }
    }
}
