using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(NativeConverter))]
    public abstract class NativeAnyOfContainer
    {
        public static NativeAnyOfContainer FromPrecision(double precision)
        {
            return new PrecisionCase().Set(precision);
        }

        public static NativeAnyOfContainer FromMString(string mString)
        {
            return string.IsNullOrEmpty(mString) ? null : new MStringCase().Set(mString);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Precision(double precision);

            T MString(string mString);
        }

        [JsonConverter(typeof(CaseConverter<PrecisionCase, double>), new JsonToken[] { JsonToken.Float })]
        private class PrecisionCase : NativeAnyOfContainer, ICaseValue<PrecisionCase, double>
        {
            private double precision;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Precision(precision);
            }

            public PrecisionCase Set(object value)
            {
                if (value is double newValue)
                {
                    precision = newValue;
                    return this;
                }
                throw new InvalidOperationException();
            }
            public double Get()
            {
                return precision;
            }

            public override string ToString()
            {
                return precision.ToString();
            }
        }

        [JsonConverter(typeof(CaseConverter<MStringCase, string>), new JsonToken[] { JsonToken.String })]
        private class MStringCase : NativeAnyOfContainer, ICaseValue<MStringCase, string>
        {
            private string mString;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.MString(mString);
            }

            public MStringCase Set(object value)
            {
                if (value is string newValue)
                {
                    mString = newValue;
                    return this;
                }
                throw new InvalidOperationException();
            }
            public string Get()
            {
                return mString;
            }

            public override string ToString()
            {
                return mString.ToString();
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
