using System;
using Newtonsoft.Json;

namespace APIMatic.Core.Utilities.Converters
{
    public class UnknownEnumConverter<T> : JsonConverter where T : JsonConverter, new()
    {
        private readonly Type _type;
        private readonly string _unknownValue;
        private readonly T _innerJsonConverter;

        public UnknownEnumConverter(Type type, string unknownValue)
        {
            _type = type;
            _unknownValue = unknownValue;
            _innerJsonConverter = new T();
        }

        public override bool CanConvert(Type objectType)
        {
            return _innerJsonConverter.CanConvert(objectType);
        }

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

            if (!Enum.IsDefined(objectType, value) && Enum.IsDefined(objectType, _unknownValue))
            {
                return Enum.Parse(objectType, _unknownValue);
            }

            return _innerJsonConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.ToString() == _unknownValue)
            {
                throw new JsonSerializationException($"{_type}.{value} is not a valid enum value for serialization!");
            }

            _innerJsonConverter.WriteJson(writer, value, serializer);
        }
    }
}
