using System;
using APIMatic.Core.Utilities.Converters.Interfaces;
using Newtonsoft.Json;

namespace APIMatic.Core.Utilities.Converters
{
    public class UnknownEnumConverter<T> : JsonConverter where T : IEnumConverter, new()
    {
        private readonly string _unknownValue;
        private readonly T _innerJsonConverter;

        public UnknownEnumConverter(string unknownValue)
        {
            _unknownValue = unknownValue ?? throw new ArgumentNullException(nameof(unknownValue));
            _innerJsonConverter = new T();
        }

        public override bool CanConvert(Type objectType)
        {
            var enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return enumType.IsEnum && Enum.IsDefined(enumType, _unknownValue) && _innerJsonConverter.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            try
            {
                return _innerJsonConverter.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (JsonSerializationException)
            {
                var enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;
                return Enum.Parse(enumType, _unknownValue);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var objectType = value.GetType();
            var enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;

            if (Enum.GetName(enumType, value) == _unknownValue)
            {
                throw new JsonSerializationException($"{enumType.FullName}.{value} is not a valid enum value for serialization!");
            }

            _innerJsonConverter.WriteJson(writer, value, serializer);
        }
    }
}
