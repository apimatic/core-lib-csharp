using System;
using System.Collections.Generic;
using System.Linq;
using APIMatic.Core.Types.Sdk.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities.Converters
{
    public class UnionTypeConverter<T> : JsonConverter<T>
    {
        private readonly List<UnionType> _types;
        private readonly bool _isOneOf;
        private readonly string _discriminator;

        public UnionTypeConverter(Type[] types, bool isOneOf)
        {
            _types = PopulateUnionTypes(types, null);
            _isOneOf = isOneOf;
            _discriminator = null;
        }

        public UnionTypeConverter(Type[] types, string[] discriminatorValues, string discriminator, bool isOneOf)
        {
            _types = PopulateUnionTypes(types, discriminatorValues);
            _discriminator = discriminator;
            _isOneOf = isOneOf;
        }

        private List<UnionType> PopulateUnionTypes(Type[] types, string[] discriminatorValues)
        {
            return types
                .Select((type, index) => new UnionType
                {
                    Type = type,
                    DiscriminatorValue = discriminatorValues?.ElementAtOrDefault(index)
                })
                .ToList();
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var types = _types;
            if (_discriminator != null && token.Type == JTokenType.Object)
            {
                var discriminatorValue = token[_discriminator]?.Value<string>();
                if (discriminatorValue != null)
                {
                    types = _types.Where(type => type.DiscriminatorValue == discriminatorValue).ToList();
                }
            }

            return DeserializeOneOfAnyOf(token, serializer, types);
        }

        private T DeserializeOneOfAnyOf(JToken token, JsonSerializer serializer, List<UnionType> types)
        {
            var mappedValues = new List<(string dataType, T value)>();
            var unMappedTypes = new List<string>();

            foreach (var (type, dataType) in types.Select(type => (type, GetFieldType(type.Type))))
            {
                TryDeserializer(() => serializer.Deserialize(token.CreateReader(), type.Type), dataType, mappedValues, unMappedTypes);

                if (!_isOneOf && mappedValues.Any())
                {
                    return mappedValues[0].value;
                }

                if (mappedValues.Count > 1)
                {
                    throw new OneOfValidationException(mappedValues[0].dataType, mappedValues[1].dataType, token.ToString());
                }
            }

            if (mappedValues.Any())
            {
                return mappedValues[0].value;
            }

            if (token.Type == JTokenType.Null)
            {
                // allow null values to pass as the OAF containers are nullable by default
                return default;
            }

            if (_isOneOf)
            {
                throw new OneOfValidationException(unMappedTypes, token.ToString());
            }
            throw new AnyOfValidationException(unMappedTypes, token.ToString());

        }

        private void TryDeserializer(Func<object> deserializer, string dataType, List<(string dataType, T value)> mappedTypes, List<string> unMappedTypes)
        {
            try
            {
                object deserializedObject = deserializer();
                mappedTypes.Add((dataType, (T)deserializedObject));
            }
            catch (JsonSerializationException)
            {
                unMappedTypes.Add(dataType);
            }
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private string GetFieldType(Type type) => type.GetFields()[0].FieldType.Name.ToLower();
    }

    internal class UnionType
    {
        public Type Type { get; set; }
        public string DiscriminatorValue { get; set; }
    }
}
