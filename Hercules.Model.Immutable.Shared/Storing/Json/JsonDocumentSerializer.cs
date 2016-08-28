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

        static JsonDocumentSerializer()
        {
            HistorySerializerSettings.Converters.Add(new PropertiesBagConverter());
            HistorySerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            HistorySerializerSettings.Formatting = Formatting.Indented;
        }

        public static byte[] Serialize(JsonHistory history)
        {
            Guard.NotNull(history, nameof(history));

            using (MemoryStream stream = new MemoryStream())
            {
                Serialize(history, stream);

                return stream.ToArray();
            }
        }

        public static void Serialize(JsonHistory history, Stream stream)
        {
            Guard.NotNull(history, nameof(history));
            Guard.NotNull(stream, nameof(stream));

            JsonStreamConvert.SerializeAsJson(history, stream, HistorySerializerSettings);
        }

        public static Document Deserialize(byte[] contents)
        {
            Guard.NotNull(contents, nameof(contents));

            using (MemoryStream stream = new MemoryStream(contents))
            {
                return Deserialize(stream);
            }
        }

        public static Document Deserialize(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            JsonHistory history = JsonStreamConvert.DeserializeAsJson<JsonHistory>(stream, HistorySerializerSettings);

            return history.ToDocument();
        }
    }
}
