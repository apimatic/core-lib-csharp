using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using APIMatic.Core.Utilities.Converters;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<NativeAnyOfContainer>),
        new Type[] {
            typeof(PrecisionCase),
            typeof(StringCase)
        },
        false
    )]
    public abstract class NativeAnyOfContainer
    {
        public static NativeAnyOfContainer FromPrecision(double value)
        {
            return new PrecisionCase().Set(value);
        }

        public static NativeAnyOfContainer FromString(string value)
        {
            return string.IsNullOrEmpty(value) ? null : new StringCase().Set(value);
        }

        public abstract T Match<T>(Func<double, T> precision, Func<string, T> mString);

        [JsonConverter(typeof(UnionTypeCaseConverter<PrecisionCase, double>), JTokenType.Float)]
        private sealed class PrecisionCase : NativeAnyOfContainer, ICaseValue<PrecisionCase, double>
        {
            public double value;

            public override T Match<T>(Func<double, T> precision, Func<string, T> mString)
            {
                return precision(value);
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

        [JsonConverter(typeof(UnionTypeCaseConverter<StringCase, string>), JTokenType.String)]
        private sealed class StringCase : NativeAnyOfContainer, ICaseValue<StringCase, string>
        {
            public string value;

            public override T Match<T>(Func<double, T> precision, Func<string, T> mString)
            {
                return mString(value);
            }

            public StringCase Set(string value)
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
