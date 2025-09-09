using APIMatic.Core.Utilities.Converters;
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

        public static NativeNullableOneOfContainer FromMString(string value)
        {
            return string.IsNullOrEmpty(value) ? null : new MStringCase().Set(value);
        }

        public abstract T Match<T>(Func<double?, T> precision, Func<string, T> mString);

        [JsonConverter(typeof(UnionTypeCaseConverter<PrecisionCase, double?>), JTokenType.Float, JTokenType.Null)]
        private sealed class PrecisionCase : NativeNullableOneOfContainer, ICaseValue<PrecisionCase, double?>
        {
            public double? value;

            public override T Match<T>(Func<double?, T> precision, Func<string, T> mString)
            {
                return precision(value);
            }

            public PrecisionCase Set(double? value)
            {
                this.value = value;
                return this;
            }
            public double? Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<MStringCase, string>), JTokenType.String)]
        private sealed class MStringCase : NativeNullableOneOfContainer, ICaseValue<MStringCase, string>
        {
            public string value;

            public override T Match<T>(Func<double?, T> precision, Func<string, T> mString)
            {
                return mString(value);
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
    }
}
