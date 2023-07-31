using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using APIMatic.Core.Utilities.Converters;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
         typeof(UnionTypeConverter<NativeOneOfCollectionContainer>),
         new Type[] {
            typeof(PrecisionArrayCase),
            typeof(MStringArrayCase),
            typeof(CustomTypeDictionaryCase)
         },
         true
     )]
    public abstract class NativeOneOfCollectionContainer
    {
        public static NativeOneOfCollectionContainer FromPrecisionArray(double[] value)
        {
            return new PrecisionArrayCase().Set(value);
        }

        public static NativeOneOfCollectionContainer FromMString(string[] value)
        {
            return value == null || value.Length == 0 ? null : new MStringArrayCase().Set(value);
        }

        public static NativeOneOfCollectionContainer FromCutomTypeDictionary(Dictionary<string, Atom> customTypeDictionary)
        {
            return new CustomTypeDictionaryCase().Set(customTypeDictionary);
        }

        public abstract T Match<T>(Func<double[], T> precision, Func<string[], T> mString, Func<Dictionary<string, Atom>, T> customTypeDictionay);

        [JsonConverter(typeof(UnionTypeCaseConverter<PrecisionArrayCase, double[]>), JTokenType.Float)]
        private class PrecisionArrayCase : NativeOneOfCollectionContainer, ICaseValue<PrecisionArrayCase, double[]>
        {
            public double[] value;

            public override T Match<T>(Func<double[], T> precision, Func<string[], T> mString, Func<Dictionary<string, Atom>, T> customTypeDictionay)
            {
                return precision(value);
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

        [JsonConverter(typeof(UnionTypeCaseConverter<MStringArrayCase, string[]>), JTokenType.String)]
        private class MStringArrayCase : NativeOneOfCollectionContainer, ICaseValue<MStringArrayCase, string[]>
        {
            public string[] value;

            public override T Match<T>(Func<double[], T> precision, Func<string[], T> mString, Func<Dictionary<string, Atom>, T> customTypeDictionay)
            {
                return mString(value);
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

        [JsonConverter(typeof(UnionTypeCaseConverter<CustomTypeDictionaryCase, Dictionary<string, Atom>>))]
        private class CustomTypeDictionaryCase : NativeOneOfCollectionContainer, ICaseValue<CustomTypeDictionaryCase, Dictionary<string, Atom>>
        {
            public Dictionary<string, Atom> value;

            public override T Match<T>(Func<double[], T> precision, Func<string[], T> mString, Func<Dictionary<string, Atom>, T> customTypeDictionay)
            {
                return customTypeDictionay(value);
            }

            public CustomTypeDictionaryCase Set(Dictionary<string, Atom> value)
            {
                this.value = value;
                return this;
            }

            public Dictionary<string, Atom> Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }
    }
}
