using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using APIMatic.Core.Types.Sdk.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities
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

            if (_discriminator != null)
            {
                var discriminatorValue = token.SelectToken(_discriminator)?.Value<string>();
                if (discriminatorValue != null)
                {
                    _types.RemoveAll(type => type.DiscriminatorValue != discriminatorValue);
                }
            }

            return DeserializeOneOfAnyOf(token, serializer);
        }

        private T DeserializeOneOfAnyOf(JToken token, JsonSerializer serializer)
        {
            var mappedTypes = new List<string>();
            var unMappedTypes = new List<string>();
            object deserializedObject = null;
            var json = token.ToString();
            foreach (var (type, dataType) in _types.Select(type => (type, GetPrivateFieldType(type.Type))))
            {
                try
                {
                    deserializedObject = serializer.Deserialize(token.CreateReader(), type.Type);
                    mappedTypes.Add(dataType);

                    if (!_isOneOf)
                    {
                        return (T)deserializedObject;
                    }

                    if (mappedTypes.Count > 1)
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

        private string GetPrivateFieldType(Type type)
        {
            return type
                .GetField("value",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .FieldType.Name.ToLower();
        }
    }

    internal class UnionType
    {
        public Type Type { get; set; }
        public string DiscriminatorValue { get; set; }
    }
}
