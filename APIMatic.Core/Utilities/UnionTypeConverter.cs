using System;
using System.Collections.Generic;
using System.Reflection;
using APIMatic.Core.Types.Sdk.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities
{
    public class UnionTypeConverter<T> : JsonConverter<T>
    {
        private readonly IEnumerable<UnionType> _types;
        private readonly bool _isOneOf;
        private readonly string _discriminator;

        public UnionTypeConverter(Type[] types, bool isOneOf)
        {
            _types = PopulateUnionType(types, null);
            _isOneOf = isOneOf;
        }

        public UnionTypeConverter(Type[] types, string[] discriminatorValues, string discriminator, bool isOneOf)
        {
            _isOneOf = isOneOf;
            _discriminator = discriminator;
            _types = PopulateUnionType(types, discriminatorValues);
        }

        private IEnumerable<UnionType> PopulateUnionType(Type[] types, string[] discriminatorValues)
        {
            for (int i = 0; i < types.Length; i++)
            {
                yield return new UnionType()
                {
                    Type = types[i],
                    DiscriminatorValue = discriminatorValues?[i],
                };
            }
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.ReadFrom(reader);
            if (_discriminator != null)
            {
                token = token.SelectToken(_discriminator);
            }
            return token != null ? TryDeserializeOneOfAnyOf(token, serializer) : default;
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
                var dataType = type.Type.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance).FieldType.Name.ToLower();
                try
                {
                    deserializedObject = serializer.Deserialize(token.CreateReader(), type.Type);
                    mappedTypes.Add(dataType);
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
                    unMappedTypes.Add(dataType);
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

    internal class UnionType
    {
        public Type Type { get; set; }
        public string DiscriminatorValue { get; set; }

    }
}
