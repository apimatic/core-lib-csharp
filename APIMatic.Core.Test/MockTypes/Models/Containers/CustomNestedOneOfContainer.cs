using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;
using System;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<CustomNestedOneOfContainer>),
        new Type[] {
            typeof(AtomCase),
            typeof(OrbitCase)
        },
        true
    )]
    public abstract class CustomNestedOneOfContainer
    {
        public static CustomNestedOneOfContainer FromAtom(Atom value)
        {
            return new AtomCase().Set(value);
        }

        public static CustomNestedOneOfContainer Fromorbit(NativeOneOfContainer value)
        {
            return new OrbitCase().Set(value);
        }

        public abstract T Match<T>(Func<Atom, T> atom, Func<NativeOneOfContainer, T> nestedOneOf);

        [JsonConverter(typeof(UnionTypeCaseConverter<AtomCase, Atom>))]
        private class AtomCase : CustomNestedOneOfContainer, ICaseValue<AtomCase, Atom>
        {
            public Atom value;

            public override T Match<T>(Func<Atom, T> atom, Func<NativeOneOfContainer, T> nestedOneOf)
            {
                return atom(value);
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

        [JsonConverter(typeof(UnionTypeCaseConverter<OrbitCase, NativeOneOfContainer>))]
        private class OrbitCase : CustomNestedOneOfContainer, ICaseValue<OrbitCase, NativeOneOfContainer>
        {
            public NativeOneOfContainer value;

            public override T Match<T>(Func<Atom, T> atom, Func<NativeOneOfContainer, T> nestedOneOf)
            {
                return nestedOneOf(value);
            }

            public NativeOneOfContainer Get()
            {
                return value;
            }

            public OrbitCase Set(NativeOneOfContainer value)
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
