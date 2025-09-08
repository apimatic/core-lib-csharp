using System;
using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<NativeOneOfContainer>),
        new Type[] {
            typeof(PrecisionCase),
            typeof(MStringCase)
        },
        true
    )]
    public abstract class TestContainer
    {
        public static NativeOneOfContainer FromPrecision(double value)
        {
            return new PrecisionCase().Set(value);
        }

        public static NativeOneOfContainer FromMString(string value)
        {
            return string.IsNullOrEmpty(value) ? null : new MStringCase().Set(value);
        }

        public abstract T Match<T>(Func<double, T> precision, Func<string, T> mString);

        private sealed class PrecisionCase : NativeOneOfContainer, ICaseValue<PrecisionCase, double>
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

        private sealed class MStringCase : NativeOneOfContainer, ICaseValue<MStringCase, string>
        {
            public string value;

            public override T Match<T>(Func<double, T> precision, Func<string, T> mString)
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
