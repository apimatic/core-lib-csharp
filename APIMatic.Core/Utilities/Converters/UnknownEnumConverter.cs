using System;
using Newtonsoft.Json;

namespace APIMatic.Core.Utilities.Converters
{
    public class UnknownEnumConverter<T> : JsonConverter where T : JsonConverter, new()
    {
        private readonly string _unknownValue;
        private readonly T _innerJsonConverter;

        public UnknownEnumConverter(string unknownValue)
        {
            _unknownValue = unknownValue;
            _innerJsonConverter = new T();
        }

        public override bool CanConvert(Type objectType) => _innerJsonConverter.CanConvert(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            object value = reader.Value;
            if (reader.TokenType == JsonToken.Integer)
            {
                value = Convert.ToInt32(value);
            }

            if (!Enum.IsDefined(objectType, value))
            {
                return Enum.Parse(objectType, _unknownValue);
            }

            return _innerJsonConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumType = value.GetType();

            if (Enum.GetName(enumType, value) == _unknownValue)
            {
                throw new JsonSerializationException($"{enumType.FullName}.{value} is not a valid enum value for serialization!");
            }

            _innerJsonConverter.WriteJson(writer, value, serializer);
        }
    }
}
