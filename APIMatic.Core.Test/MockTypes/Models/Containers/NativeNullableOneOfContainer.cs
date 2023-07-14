using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<NativeNullableOneOfContainer>),
        new Type[] {
            typeof(PrecisionCase),
            typeof(MStringCase)
        },
        true
    )]
    public abstract class NativeNullableOneOfContainer
    {
        public static NativeNullableOneOfContainer FromPrecision(double value)
        {
            return new PrecisionCase().Set(value);
        }

        public static NativeNullableOneOfContainer FromMString(string? value)
        {
            return string.IsNullOrEmpty(value) ? null : new MStringCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Precision(double value);

            T MString(string? value);
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<PrecisionCase, double>), JTokenType.Float)]
        private class PrecisionCase : NativeNullableOneOfContainer, ICaseValue<PrecisionCase, double>
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

        [JsonConverter(typeof(UnionTypeCaseConverter<MStringCase, string?>), JTokenType.String, JTokenType.Null)]
        private class MStringCase : NativeNullableOneOfContainer, ICaseValue<MStringCase, string?>
        {
            private string? value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.MString(value);
            }

            public MStringCase Set(string? value)
            {
                this.value = value;
                return this;
            }

            public string? Get()
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
