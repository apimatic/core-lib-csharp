using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(NativeConverter))]
    public abstract class NativeAnyOfContainer
    {
        public static NativeAnyOfContainer FromPrecision(double value)
        {
            return new PrecisionCase().Set(value);
        }

        public static NativeAnyOfContainer FromMString(string value)
        {
            return string.IsNullOrEmpty(value) ? null : new MStringCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Precision(double value);

            T MString(string value);
        }

        [JsonConverter(typeof(CaseConverter<PrecisionCase, double>), new JTokenType[] { JTokenType.Float })]
        private class PrecisionCase : NativeAnyOfContainer, ICaseValue<PrecisionCase, double>
        {
            private double value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Precision(value);
            }

            public PrecisionCase Set(double value)
            {
                this.value = value;
                return this;
            }
            public double Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        [JsonConverter(typeof(CaseConverter<MStringCase, string>), new JTokenType[] { JTokenType.String })]
        private class MStringCase : NativeAnyOfContainer, ICaseValue<MStringCase, string>
        {
            private string value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.MString(value);
            }

            public MStringCase Set(string value)
            {
                this.value = value;
                return this;
            }
            public string Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        private class NativeConverter : JsonConverter<NativeAnyOfContainer>
        {
            public override bool CanRead => true;
            public override bool CanWrite => false;

            public override NativeAnyOfContainer ReadJson(JsonReader reader, Type objectType, NativeAnyOfContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(PrecisionCase), "precision" },
                    { typeof(MStringCase), "string" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, false);
                return deserializedObject as NativeAnyOfContainer;
            }

            public override void WriteJson(JsonWriter writer, NativeAnyOfContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
