﻿using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities
{
    public class CaseConverter<C, T> : JsonConverter<C> where C : ICaseValue<C, T>, new()
    {
        private readonly JTokenType _typeToken = JTokenType.None;

        public CaseConverter() { }

        public CaseConverter(JTokenType typeToken)
        {
            _typeToken = typeToken;
        }

        public override C ReadJson(JsonReader reader, Type objectType, C existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken token = JToken.ReadFrom(reader);
            try
            {
                T value = Deserialize(token, serializer);
                return new C().Set(value);
            }
            catch (Exception)
            {
                throw new JsonSerializationException($"Invalid {typeof(T)}");
            }
        }

        private T Deserialize(JToken token, JsonSerializer serializer)
        {
            if (_typeToken == JTokenType.None
                || VerifyInternalType(token))
            {
                return serializer.Deserialize<T>(token.CreateReader());
            }
            throw new InvalidOperationException();
        }

        private bool VerifyInternalType(JToken token)
        {
            if (_typeToken == token.Type)
            {
                return true;
            }

            if (token.Type != JTokenType.Array && token.Type != JTokenType.Object)
            {
                return false;
            }

            return token.All(tkn => VerifyInternalType(tkn));
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
}
