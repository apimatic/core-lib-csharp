using APIMatic.Core.Utilities;
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

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(Atom[] value);

            T Orbit(Orbit[] value);
        }

        [JsonConverter(typeof(CaseConverter<AtomCase, Atom[]>))]
        private class AtomCase : CustomOneOfCollectionContainer, ICaseValue<AtomCase, Atom[]>
        {
            private Atom[] value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Atom(value);
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

        [JsonConverter(typeof(CaseConverter<OrbitCase, Orbit[]>))]
        private class OrbitCase : CustomOneOfCollectionContainer, ICaseValue<OrbitCase, Orbit[]>
        {
            private Orbit[] value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Orbit(value);
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
