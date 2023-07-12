using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(NativeOneOfCollectionConverter))]
    public abstract class NativeOneOfCollectionContainer
    {
        public static NativeOneOfCollectionContainer FromPrecisionArray(double[] precision)
        {
            return new PrecisionArrayCase().Set(precision);
        }

        public static NativeOneOfCollectionContainer FromMString(string[] mString)
        {
            return mString == null || mString.Length == 0 ? null : new MStringArrayCase().Set(mString);
        }

        public static NativeOneOfCollectionContainer FromCutomTypeDictionary(Dictionary<string, Atom> customTypeDictionary)
        {
            return new CustomTypeDictionaryCase().Set(customTypeDictionary);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Precision(double[] precision);

            T MString(string[] mString);

            T CustomTypeDictionary(Dictionary<string, Atom> customTypeDictionary);
        }

        [JsonConverter(typeof(CaseConverter<PrecisionArrayCase, double[]>), new JTokenType[] { JTokenType.Float })]
        private class PrecisionArrayCase : NativeOneOfCollectionContainer, ICaseValue<PrecisionArrayCase, double[]>
        {
            private double[] precision;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Precision(precision);
            }

            public PrecisionArrayCase Set(double[] value)
            {
                precision = value;
                return this;
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

        [JsonConverter(typeof(CaseConverter<MStringArrayCase, string[]>), new JTokenType[] { JTokenType.String })]
        private class MStringArrayCase : NativeOneOfCollectionContainer, ICaseValue<MStringArrayCase, string[]>
        {
            private string[] mString;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.MString(mString);
            }

            public MStringArrayCase Set(string[] value)
            {
                mString = value;
                return this;
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

        [JsonConverter(typeof(CaseConverter<CustomTypeDictionaryCase, Dictionary<string, Atom>>))]
        private class CustomTypeDictionaryCase : NativeOneOfCollectionContainer, ICaseValue<CustomTypeDictionaryCase, Dictionary<string, Atom>>
        {
            private Dictionary<string, Atom> customTypeDict;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.CustomTypeDictionary(customTypeDict);
            }

            public CustomTypeDictionaryCase Set(Dictionary<string, Atom> value)
            {
                customTypeDict = value;
                return this;
            }

            public Dictionary<string, Atom> Get()
            {
                return customTypeDict;
            }

            public override string ToString()
            {
                return customTypeDict.ToString();
            }
        }

        private class NativeOneOfCollectionConverter : JsonConverter<NativeOneOfCollectionContainer>
        {
            public override bool CanRead => true;
            public override bool CanWrite => false;

            public override NativeOneOfCollectionContainer ReadJson(JsonReader reader, Type objectType, NativeOneOfCollectionContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(PrecisionArrayCase), "precision" },
                    { typeof(MStringArrayCase), "string" },
                    { typeof(CustomTypeDictionaryCase), "customDictionary" },
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, true);
                return deserializedObject as NativeOneOfCollectionContainer;
            }

            public override void WriteJson(JsonWriter writer, NativeOneOfCollectionContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
