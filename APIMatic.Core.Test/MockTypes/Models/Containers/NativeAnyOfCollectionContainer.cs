using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(NativeAnyOfCollectionConverter))]
    public abstract class NativeAnyOfCollectionContainer
    {
        public static NativeAnyOfCollectionContainer FromPrecisionArray(double[] value)
        {
            return new PrecisionArrayCase().Set(value);
        }

        public static NativeAnyOfCollectionContainer FromMString(string[] value)
        {
            return value == null || value.Length == 0 ? null : new MStringArrayCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<T>
        {
            T Precision(double[] value);

            T MString(string[] value);
        }

        [JsonConverter(typeof(CaseConverter<PrecisionArrayCase, double[]>), new JTokenType[] { JTokenType.Float })]
        private class PrecisionArrayCase : NativeAnyOfCollectionContainer, ICaseValue<PrecisionArrayCase, double[]>
        {
            private double[] value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Precision(value);
            }

            public PrecisionArrayCase Set(double[] value)
            {
                this.value = value;
                return this;
            }

            public double[] Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        [JsonConverter(typeof(CaseConverter<MStringArrayCase, string[]>), new JTokenType[] { JTokenType.String })]
        private class MStringArrayCase : NativeAnyOfCollectionContainer, ICaseValue<MStringArrayCase, string[]>
        {
            private string[] value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.MString(value);
            }

            public MStringArrayCase Set(string[] value)
            {
                this.value = value;
                return this;
            }

            public string[] Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
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
