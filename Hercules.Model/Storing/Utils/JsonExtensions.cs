// ==========================================================================
// JsonExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace Hercules.Model.Storing.Utils
{
    internal static class JsonExtensions
    {
        private static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings();

        public static async Task SerializeAsJsonAsync<T>(this T value, StorageFile file, JsonSerializerSettings settings = null)
        {
            using (Stream fileStream = await file.OpenStreamForWriteAsync())
            {
                value.SerializeAsJson(fileStream, settings ?? DefaultSettings);
            }
        }

        public static void SerializeAsJson<T>(this T value, Stream stream, JsonSerializerSettings settings = null)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
            {
                WriteObject(settings ?? DefaultSettings, writer, value);

                writer.Flush();
            }
        }

        public static T DeserializeAsJson<T>(this Stream stream, JsonSerializerSettings settings = null)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return ReadObject<T>(settings ?? DefaultSettings, reader);
            }
        }

        private static void WriteObject(JsonSerializerSettings settings, TextWriter writer, object value)
        {
            using (JsonWriter jsonWriter = CreateJsonWriter(writer))
            {
                CreateJsonSerializer(settings).Serialize(jsonWriter, value);
            }
        }

        private static T ReadObject<T>(JsonSerializerSettings settings, TextReader reader)
        {
            using (JsonReader jsonReader = CreateJsonReader(reader))
            {
                return CreateJsonSerializer(settings).Deserialize<T>(jsonReader);
            }
        }

        private static JsonWriter CreateJsonWriter(TextWriter textWriter)
        {
            JsonTextWriter writer = new JsonTextWriter(textWriter) { CloseOutput = false };

            return writer;
        }

        private static JsonReader CreateJsonReader(TextReader textReader)
        {
            JsonTextReader reader = new JsonTextReader(textReader);

            return reader;
        }

        private static JsonSerializer CreateJsonSerializer(JsonSerializerSettings settings)
        {
            JsonSerializer serializer = JsonSerializer.Create(settings);

            return serializer;
        }
    }
}
