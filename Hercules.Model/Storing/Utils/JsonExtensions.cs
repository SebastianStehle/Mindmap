// ==========================================================================
// JsonExtensions.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Hercules.Model.Storing.Utils
{
    internal static class JsonExtensions
    {
        private static readonly JsonSerializerSettings defaultSettings = new JsonSerializerSettings();

        public static void SerializeAsJsonToStream<T>(this T value, Stream stream, JsonSerializerSettings settings = null)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 4096, true) { AutoFlush = true })
            {
                WriteObject(settings ?? defaultSettings, writer, value);

                writer.Flush();
            }
        }
        

        public static T DeserializeAsJsonFromStream<T>(this Stream stream, JsonSerializerSettings settings = null)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return ReadObject<T>(settings ?? defaultSettings, reader);
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
            JsonTextReader Reader = new JsonTextReader(textReader);

            return Reader;
        }

        private static JsonSerializer CreateJsonSerializer(JsonSerializerSettings settings)
        {
            JsonSerializer serializer = JsonSerializer.Create(settings);

            return serializer;
        }
    }
}
