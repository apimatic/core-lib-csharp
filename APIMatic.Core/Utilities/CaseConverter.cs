using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace APIMatic.Core.Utilities
{
    public class CaseConverter<C, T> : JsonConverter<C> where C : ICaseValue<C, T>, new()
    {
        private readonly IEnumerable<JsonToken> _typeTokens = null;
        private readonly bool _isNativeCollection = false;

        public CaseConverter() { }

        public CaseConverter(JsonToken[] typeTokens)
        {
            if (typeTokens.Contains(JsonToken.StartArray))
            {
                _isNativeCollection = true;
            }
            _typeTokens = typeTokens.Any() ? typeTokens : null;
        }

        public override C ReadJson(JsonReader reader, Type objectType, C existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                object value = _isNativeCollection ? DeserializeCollection(reader) : Deserialize(reader, serializer);
                return new C().Set(value);
            }
            catch (Exception)
            {
                throw new JsonSerializationException($"Invalid {typeof(T)}");
            }
        }

        private object Deserialize(JsonReader reader, JsonSerializer serializer)
        {
            if (_typeTokens == null || _typeTokens.Contains(reader.TokenType))
            {
                return serializer.Deserialize<T>(reader);
            }
            throw new InvalidOperationException();
        }

        private List<object> DeserializeCollection(JsonReader reader)
        {
            if (_typeTokens == null || reader.TokenType != JsonToken.StartArray)
            {
                throw new InvalidOperationException();
            }

            var collection = new List<object>();
            while (reader.Read())
            {
                if (_typeTokens.Contains(reader.TokenType))
                {
                    collection.Add(reader.Value);
                }
                else if (reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            return collection;
        }

        public override void WriteJson(JsonWriter writer, C value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Get());
        }
    }

    public interface ICaseValue<out C, out T>
    {
        T Get();

        C Set(object value);
    }
}
