using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using System;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<CustomOneOfWithDiscContainer>),
        new Type[] {
            typeof(AtomCase),
            typeof(HeliumCase)
        },
        new string[]
        {
            "12",
            "3"
        },
        "NumberOfElectrons",
        true
    )]
    public abstract class CustomOneOfWithDiscContainer
    {
        public static CustomOneOfWithDiscContainer FromAtom(Atom value)
        {
            return new AtomCase().Set(value);
        }

        public static CustomOneOfWithDiscContainer FromHelium(Helium value)
        {
            return new HeliumCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(Atom value);

            T Helium(Helium value);
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<AtomCase, Atom>))]
        private class AtomCase : CustomOneOfWithDiscContainer, ICaseValue<AtomCase, Atom>
        {
            public Atom value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Atom(value);
            }

            public AtomCase Set(Atom value)
            {
                this.value = value;
                return this;
            }

            public Atom Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<HeliumCase, Helium>))]
        private class HeliumCase : CustomOneOfWithDiscContainer, ICaseValue<HeliumCase, Helium>
        {
            public Helium value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Helium(value);
            }

            public Helium Get()
            {
                return value;
            }

            public HeliumCase Set(Helium value)
            {
                this.value = value;
                return this;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }
    }
}
