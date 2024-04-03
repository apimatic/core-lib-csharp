using System;
using APIMatic.Core.Utilities.Converters.Interfaces;
using Newtonsoft.Json;

namespace APIMatic.Core.Utilities.Converters
{
    public class NumberEnumConverter : JsonConverter, IEnumConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var innerType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            if (CanConvert(innerType) && reader.TokenType == JsonToken.Integer)
            {
                var enumValue = Enum.ToObject(innerType, Convert.ToInt32(reader.Value));
                return enumValue;
            }

            throw new JsonSerializationException($"Invalid type for number enum {innerType.FullName}.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
