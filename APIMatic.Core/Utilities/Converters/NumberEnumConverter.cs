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

        private Type GetInnerType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GenericTypeArguments[0];
            }

            return type;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var innerType = GetInnerType(objectType);
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
