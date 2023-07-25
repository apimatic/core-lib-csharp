using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIMatic.Core.Utilities
{
    public class EnumCaseConverter<T> : StringEnumConverter where T : Enum
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string value = (string)reader.Value;
                return Enum.Parse(typeof(T), value, true);
            }

            throw new JsonSerializationException($"Invalid {typeof(T)}.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // No need to implement
        }
    }

}
