using Newtonsoft.Json;
using System;
using APIMatic.Core.Utilities;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
       typeof(UnionTypeConverter<NativeOneOfContainer>),
       new Type[] {
            typeof(PrecisionArrayCase),
            typeof(MStringArrayCase)
       },
       new string[] {
            "discVal1",
            "discVal2"
       },
       "discriminatorField",
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
    }
}
