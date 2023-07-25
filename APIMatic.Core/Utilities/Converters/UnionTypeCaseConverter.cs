using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities.Converters
{
    public class UnionTypeCaseConverter<C, T> : JsonConverter<C> where C : ICaseValue<C, T>, new()
    {
        private readonly JTokenType _typeToken = JTokenType.None;
        private readonly JTokenType _nullableToken = JTokenType.None;

        public UnionTypeCaseConverter() { }

        public UnionTypeCaseConverter(JTokenType typeToken)
        {
            _typeToken = typeToken;
        }

        public UnionTypeCaseConverter(JTokenType typeToken, JTokenType jTokenType)
        {
            _typeToken = typeToken;
            _nullableToken = jTokenType;
        }

        public override C ReadJson(JsonReader reader, Type objectType, C existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.ReadFrom(reader);
            try
            {
                var typeObject = new C();
                if (typeof(C).GetInterfaces().Contains(typeof(ICustomConverter)))
                {
                    serializer.Converters.Clear();
                    serializer.Converters.Add(((ICustomConverter)typeObject).GetJsonConverter());
                }
                var value = Deserialize(token, serializer);
                return typeObject.Set(value);
            }
            catch (Exception)
            {
                throw new JsonSerializationException($"Invalid {typeof(T)} on value: {reader.Value}");
            }
        }

        private T Deserialize(JToken token, JsonSerializer serializer)
        {
            if (_typeToken == JTokenType.None || VerifyInternalType(token))
            {
                return serializer.Deserialize<T>(token.CreateReader());
            }
            throw new InvalidOperationException();
        }

        private bool VerifyInternalType(JToken token)
        {
            if (_typeToken == token.Type || token.Type == _nullableToken)
            {
                return true;
            }

            var containerTypes = new List<JTokenType> {
                JTokenType.Array, JTokenType.Object, JTokenType.Property
            };

            if (containerTypes.Contains(token.Type))
            {
                return token.All(tkn => VerifyInternalType(tkn));
            }

            return false;
        }

        public override void WriteJson(JsonWriter writer, C value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Get());
        }
    }

    public interface ICaseValue<out C, T>
    {
        T Get();

        C Set(T value);
    }

    public interface ICustomConverter
    {
        JsonConverter GetJsonConverter();
    }
}
