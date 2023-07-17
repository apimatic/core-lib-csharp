using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using System;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<CustomAnyOfContainer>),
        new Type[] {
            typeof(AtomCase),
            typeof(OrbitCase)
        },
        false
    )]
    public abstract class CustomAnyOfContainer
    {
        public static CustomAnyOfContainer FromAtom(Atom value)
        {
            return new AtomCase().Set(value);
        }

        public static CustomAnyOfContainer FromOrbit(Orbit value)
        {
            return new OrbitCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(Atom value);

            T Orbit(Orbit value);
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<AtomCase, Atom>))]
        private class AtomCase : CustomAnyOfContainer, ICaseValue<AtomCase, Atom>
        {
            public Atom value;

            public AtomCase Set(Atom value)
            {
                this.value = value;
                return this;
            }
            public Atom Get()
            {
                return value;
            }

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Atom(value);
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<OrbitCase, Orbit>))]
        private class OrbitCase : CustomAnyOfContainer, ICaseValue<OrbitCase, Orbit>
        {
            public Orbit value;

            public Orbit Get()
            {
                return value;
            }

            public OrbitCase Set(Orbit value)
            {
                this.value = value;
                return this;
            }

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Orbit(value);
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }
    }
}
