// ==========================================================================
// PropertiesBagConverter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System;
using Newtonsoft.Json;

namespace Hercules.Model.Storing.Json
{
    public sealed class PropertiesBagConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PropertiesBag);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            PropertiesBag properties = new PropertiesBag();

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                var key = reader.Value.ToString();

                reader.Read();

                var val = reader.Value;

                properties.Set(key, val);
            }

            return properties;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            PropertiesBag properties = (PropertiesBag)value;

            writer.WriteStartObject();

            foreach (var kvp in properties.Properties)
            {
                writer.WritePropertyName(kvp.Key);
                writer.WriteValue(kvp.Value.RawValue);
            }

            writer.WriteEndObject();
        }
    }
}
