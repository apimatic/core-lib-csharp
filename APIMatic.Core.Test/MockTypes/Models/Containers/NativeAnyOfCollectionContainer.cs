using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities;
using System.Linq;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(NativeAnyOfCollectionConverter))]
    public abstract class NativeAnyOfCollectionContainer
    {
        public static NativeAnyOfCollectionContainer FromPrecisionArray(double[] precision)
        {
            return new PrecisionArrayCase().Set(precision);
        }

        public static NativeAnyOfCollectionContainer FromMString(string[] mString)
        {
            return mString == null || mString.Length == 0 ? null : new MStringArrayCase().Set(mString);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<T>
        {
            T Precision(double[] precision);

            T MString(string[] mString);
        }

        [JsonConverter(typeof(CaseConverter<PrecisionArrayCase, double[]>), new JsonToken[] { JsonToken.Float, JsonToken.StartArray })]
        private class PrecisionArrayCase : NativeAnyOfCollectionContainer, ICaseValue<PrecisionArrayCase, double[]>
        {
            private double[] precision;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Precision(precision);
            }

            public PrecisionArrayCase Set(object value)
            {
                if (value is List<object> newValue)
                {
                    precision = newValue.Cast<double>().ToArray();
                    return this;
                }
                throw new InvalidOperationException();
            }

            public double[] Get()
            {
                return precision;
            }

            public override string ToString()
            {
                return precision.ToString();
            }
        }

        [JsonConverter(typeof(CaseConverter<MStringArrayCase, string[]>), new JsonToken[] { JsonToken.String, JsonToken.StartArray })]
        private class MStringArrayCase : NativeAnyOfCollectionContainer, ICaseValue<MStringArrayCase, string[]>
        {
            private string[] mString;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.MString(mString);
            }

            public MStringArrayCase Set(object value)
            {
                if (value is List<object> newValue)
                {
                    mString = newValue.Cast<string>().ToArray();
                    return this;
                }
                throw new InvalidOperationException();
            }

            public string[] Get()
            {
                return mString;
            }

            public override string ToString()
            {
                return mString.ToString();
            }
        }

        private class NativeAnyOfCollectionConverter : JsonConverter<NativeAnyOfCollectionContainer>
        {
            public override NativeAnyOfCollectionContainer ReadJson(JsonReader reader, Type objectType, NativeAnyOfCollectionContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(PrecisionArrayCase), "precision" },
                    { typeof(MStringArrayCase), "string" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, false);
                return deserializedObject as NativeAnyOfCollectionContainer;
            }

            public override void WriteJson(JsonWriter writer, NativeAnyOfCollectionContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
