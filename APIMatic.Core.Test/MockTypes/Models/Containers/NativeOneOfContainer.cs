using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

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
    public abstract class NativeOneOfContainer
    {
        public static NativeOneOfContainer FromPrecision(double value)
        {
            return new PrecisionCase().Set(value);
        }

        public static NativeOneOfContainer FromMString(string value)
        {
            return string.IsNullOrEmpty(value) ? null : new MStringCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Precision(double value);

            T MString(string value);
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<PrecisionCase, double>), JTokenType.Float)]
        private class PrecisionCase : NativeOneOfContainer, ICaseValue<PrecisionCase, double>
        {
            public double value;

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

        [JsonConverter(typeof(UnionTypeCaseConverter<MStringCase, string>), JTokenType.String)]
        private class MStringCase : NativeOneOfContainer, ICaseValue<MStringCase, string>
        {
            public string value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.MString(value);
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
