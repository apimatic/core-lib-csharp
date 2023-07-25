using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using APIMatic.Core.Utilities.Converters;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<ArrayOfMapContainer>),
        new Type[] {
            typeof(AtomCase),
            typeof(OrbitCase)
        },
        false
    )]
    public abstract class ArrayOfMapContainer
    {
        public static ArrayOfMapContainer FromAtom(List<Dictionary<string, Atom>> value)
        {
            return new AtomCase().Set(value);
        }

        public static ArrayOfMapContainer Fromorbit(List<Dictionary<string, Orbit>> value)
        {
            return new OrbitCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(List<Dictionary<string, Atom>> value);
            T Orbit(List<Dictionary<string, Orbit>> value);
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<AtomCase, List<Dictionary<string, Atom>>>))]
        private class AtomCase : ArrayOfMapContainer, ICaseValue<AtomCase, List<Dictionary<string, Atom>>>
        {
            public List<Dictionary<string, Atom>> value;

            public AtomCase Set(List<Dictionary<string, Atom>> value)
            {
                this.value = value;
                return this;
            }
            public List<Dictionary<string, Atom>> Get()
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

        [JsonConverter(typeof(UnionTypeCaseConverter<OrbitCase, List<Dictionary<string, Orbit>>>))]
        private class OrbitCase : ArrayOfMapContainer, ICaseValue<OrbitCase, List<Dictionary<string, Orbit>>>
        {
            public List<Dictionary<string, Orbit>> value;

            public List<Dictionary<string, Orbit>> Get()
            {
                return value;
            }

            public OrbitCase Set(List<Dictionary<string, Orbit>> value)
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
