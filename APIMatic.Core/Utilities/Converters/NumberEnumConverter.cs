using System;
using Newtonsoft.Json;

namespace APIMatic.Core.Utilities.Converters
{
    public class NumberEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.BaseType.FullName == "System.Enum";
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (CanConvert(objectType) && reader.TokenType == JsonToken.Integer)
            {
                var value = Convert.ToInt32(reader.Value);
                return value;
            }

            throw new JsonSerializationException($"Invalid type for number enum.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
