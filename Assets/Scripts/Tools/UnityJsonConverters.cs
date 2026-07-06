using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Tools
{
    public static class UnityJsonConverters
    {
        private static JsonSerializerSettings _settings;
        
        public static JsonSerializerSettings Settings => _settings ??= new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> { new ColorConverter(), new Vector2Converter() }
        };
    }

    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(value.r);
            writer.WritePropertyName("g");
            writer.WriteValue(value.g);
            writer.WritePropertyName("b");
            writer.WriteValue(value.b);
            writer.WritePropertyName("a");
            writer.WriteValue(value.a);
            writer.WriteEndObject();
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            float r = 0, g = 0, b = 0, a = 1;
            if (reader.TokenType == JsonToken.StartObject)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndObject)
                        break;

                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value.ToString().ToLower();
                        if (!reader.Read()) break;

                        if (reader.Value != null)
                        {
                            float val = Convert.ToSingle(reader.Value);
                            if (propertyName == "r") r = val;
                            else if (propertyName == "g") g = val;
                            else if (propertyName == "b") b = val;
                            else if (propertyName == "a") a = val;
                        }
                    }
                }
            }
            return new Color(r, g, b, a);
        }
    }

    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            float x = 0, y = 0;
            if (reader.TokenType == JsonToken.StartObject)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndObject)
                        break;

                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value.ToString().ToLower();
                        if (!reader.Read()) break;

                        if (reader.Value != null)
                        {
                            float val = Convert.ToSingle(reader.Value);
                            if (propertyName == "x") x = val;
                            else if (propertyName == "y") y = val;
                        }
                    }
                }
            }
            return new Vector2(x, y);
        }
    }
}
