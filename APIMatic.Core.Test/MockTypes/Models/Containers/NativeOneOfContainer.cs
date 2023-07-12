using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{

    [JsonConverter(typeof(NativeConverter))]
    public abstract class NativeOneOfContainer
    {
        public static NativeOneOfContainer FromPrecision(double precision)
        {
            return new PrecisionCase().Set(precision);
        }

        public static NativeOneOfContainer FromMString(string mString)
        {
            return string.IsNullOrEmpty(mString) ? null : new MStringCase().Set(mString);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Precision(double precision);

            T MString(string mString);
        }

        [JsonConverter(typeof(CaseConverter<PrecisionCase, double>), new JTokenType[] { JTokenType.Float })]
        private class PrecisionCase : NativeOneOfContainer, ICaseValue<PrecisionCase, double>
        {
            private double precision;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Precision(precision);
            }

            public PrecisionCase Set(double value)
            {
                precision = value;
                return this;
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

        [JsonConverter(typeof(CaseConverter<MStringCase, string>), new JTokenType[] { JTokenType.String })]
        private class MStringCase : NativeOneOfContainer, ICaseValue<MStringCase, string>
        {
            private string mString;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.MString(mString);
            }

            public MStringCase Set(string value)
            {
                mString = value;
                return this;
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

        private class NativeConverter : JsonConverter<NativeOneOfContainer>
        {
            public override bool CanRead => true;
            public override bool CanWrite => false;

            public override NativeOneOfContainer ReadJson(JsonReader reader, Type objectType, NativeOneOfContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(PrecisionCase), "precision" },
                    { typeof(MStringCase), "string" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, true);
                return deserializedObject as NativeOneOfContainer;
            }

            public override void WriteJson(JsonWriter writer, NativeOneOfContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
