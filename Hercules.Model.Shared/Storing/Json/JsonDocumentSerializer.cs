// ==========================================================================
// JsonDocumentSerializer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using GP.Utils;
using Hercules.Model.Storing.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            HistorySerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            HistorySerializerSettings.Formatting = Formatting.Indented;
        }

        public static void SerializeToStream(Stream stream, Document document)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNull(document, nameof(document));

            JsonHistory history = new JsonHistory(document);

            JsonStreamConvert.SerializeAsJson(history, stream, HistorySerializerSettings);
        }

        public static void SerializeToStream(Stream stream, JsonHistory history)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNull(history, nameof(history));

            JsonStreamConvert.SerializeAsJson(history, stream, HistorySerializerSettings);
        }

        public static JsonHistory DeserializeHistoryFromStream(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            JsonHistory history = JsonStreamConvert.DeserializeAsJson<JsonHistory>(stream, HistorySerializerSettings);

            return history;
        }

        public static Document DeserializeDocumentFromStream(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            JsonHistory history = JsonStreamConvert.DeserializeAsJson<JsonHistory>(stream, HistorySerializerSettings);

            return history.ToDocument();
        }
    }
}
