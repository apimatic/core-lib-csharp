using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using APIMatic.Core.Utilities.Converters;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
       typeof(UnionTypeConverter<NativeAnyOfCollectionContainer>),
       new Type[] {
            typeof(PrecisionArrayCase),
            typeof(MStringArrayCase)
       },
       false
   )]
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

        public abstract T Match<T>(Func<double[], T> precisionArray, Func<string[], T> mStringArray);

        [JsonConverter(typeof(UnionTypeCaseConverter<PrecisionArrayCase, double[]>), JTokenType.Float)]
        private sealed class PrecisionArrayCase : NativeAnyOfCollectionContainer, ICaseValue<PrecisionArrayCase, double[]>
        {
            public double[] value;

            public override T Match<T>(Func<double[], T> precisionArray, Func<string[], T> mStringArray)
            {
                return precisionArray(value);
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
        private sealed class MStringArrayCase : NativeAnyOfCollectionContainer, ICaseValue<MStringArrayCase, string[]>
        {
            public string[] value;

            public override T Match<T>(Func<double[], T> precisionArray, Func<string[], T> mStringArray)
            {
                return mStringArray(value);
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
    }
}
