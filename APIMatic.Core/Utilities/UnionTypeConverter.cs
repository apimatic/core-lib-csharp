using System;
using System.Collections.Generic;
using APIMatic.Core.Types.Sdk.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities
{
    public class UnionTypeConverter<T> : JsonConverter<T>
    {
        private readonly IEnumerable<Type> _types;
        private readonly bool _isOneOf;

        public UnionTypeConverter(Type[] types, bool isOneOf)
        {
            _types = types;
            _isOneOf = isOneOf;
        }

        public UnionTypeConverter(Type[] types, string[] discriminatorValues, string discriminator, bool isOneOf)
        {
            _types = types;
            _isOneOf = isOneOf;
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //
            return TryDeserializeOneOfAnyOf(JToken.ReadFrom(reader), serializer);
        }

        private T TryDeserializeOneOfAnyOf(JToken token, JsonSerializer serializer)
        {
            if (token.Type == JTokenType.Null)
            {
                return default;
            }
            List<string> mappedTypes = new List<string>();
            List<string> unMappedTypes = new List<string>();
            object deserializedObject = null;
            var json = token.ToString();
            foreach (var type in _types)
            {
                try
                {
                    deserializedObject = serializer.Deserialize(token.CreateReader(), type);
                    mappedTypes.Add(nameof(type));
                    if (!_isOneOf)
                    {
                        return (T)deserializedObject;
                    }

                    if (_isOneOf && mappedTypes.Count > 1)
                    {
                        throw new OneOfValidationException(mappedTypes[0], mappedTypes[1], json);
                    }

                }
                catch (JsonSerializationException)
                {
                    unMappedTypes.Add(nameof(type));
                }
            }

            if (mappedTypes.Count == 0)
            {
                if (!_isOneOf)
                {
                    throw new AnyOfValidationException(unMappedTypes, json);
                }
                else
                {
                    throw new OneOfValidationException(unMappedTypes, json);
                }
            }

            return (T)deserializedObject;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
