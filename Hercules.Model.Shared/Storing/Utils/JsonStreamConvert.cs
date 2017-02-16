// ==========================================================================
// JsonStreamConvert.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Hercules.Model.Storing.Utils
{
    public static class JsonStreamConvert
    {
        private static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings();

        public static void SerializeAsJson<T>(T value, Stream stream, JsonSerializerSettings settings = null)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
            {
                WriteObject(settings ?? DefaultSettings, writer, value);

                writer.Flush();
            }
        }

        public static T DeserializeAsJson<T>(Stream stream, JsonSerializerSettings settings = null)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return ReadObject<T>(settings ?? DefaultSettings, reader);
            }
        }

        private static void WriteObject(JsonSerializerSettings settings, TextWriter writer, object value)
        {
            using (var jsonWriter = CreateJsonWriter(writer))
            {
                CreateJsonSerializer(settings).Serialize(jsonWriter, value);
            }
        }

        private static T ReadObject<T>(JsonSerializerSettings settings, TextReader reader)
        {
            using (var jsonReader = CreateJsonReader(reader))
            {
                return CreateJsonSerializer(settings).Deserialize<T>(jsonReader);
            }
        }

        private static JsonWriter CreateJsonWriter(TextWriter textWriter)
        {
            var writer = new JsonTextWriter(textWriter) { CloseOutput = false };

            return writer;
        }

        private static JsonReader CreateJsonReader(TextReader textReader)
        {
            var reader = new JsonTextReader(textReader);

            return reader;
        }

        private static JsonSerializer CreateJsonSerializer(JsonSerializerSettings settings)
        {
            var serializer = JsonSerializer.Create(settings);

            return serializer;
        }
    }
}
