using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;
using System;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<CustomOneOfCollectionContainer>),
        new Type[] {
            typeof(AtomCase),
            typeof(OrbitCase)
        },
        true
    )]
    public abstract class CustomOneOfCollectionContainer
    {
        public static CustomOneOfCollectionContainer FromAtom(Atom[] value)
        {
            return new AtomCase().Set(value);
        }

        public static CustomOneOfCollectionContainer Fromorbit(Orbit[] value)
        {
            return new OrbitCase().Set(value);
        }

        public abstract T Match<T>(Func<Atom[], T> atom, Func<Orbit[], T> orbit);

        [JsonConverter(typeof(UnionTypeCaseConverter<AtomCase, Atom[]>))]
        private class AtomCase : CustomOneOfCollectionContainer, ICaseValue<AtomCase, Atom[]>
        {
            public Atom[] value;

            public override T Match<T>(Func<Atom[], T> atom, Func<Orbit[], T> orbit)
            {
                return atom(value);
            }

            public AtomCase Set(Atom[] value)
            {
                this.value = value;
                return this;
            }
            public Atom[] Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<OrbitCase, Orbit[]>))]
        private class OrbitCase : CustomOneOfCollectionContainer, ICaseValue<OrbitCase, Orbit[]>
        {
            public Orbit[] value;

            public override T Match<T>(Func<Atom[], T> atom, Func<Orbit[], T> orbit)
            {
                return orbit(value);
            }

            public Orbit[] Get()
            {
                return value;
            }

            public OrbitCase Set(Orbit[] value)
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
