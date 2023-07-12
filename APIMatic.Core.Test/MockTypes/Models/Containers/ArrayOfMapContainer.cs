using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(ArrayOfMapConverter))]
    public abstract class ArrayOfMapContainer
    {
        public static ArrayOfMapContainer FromAtom(List<Dictionary<string, Atom>> atoms)
        {
            return new AtomCase().Set(atoms);
        }

        public static ArrayOfMapContainer Fromorbit(List<Dictionary<string, Orbit>> orbits)
        {
            return new OrbitCase().Set(orbits);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(List<Dictionary<string, Atom>> atoms);

            T Orbit(List<Dictionary<string, Orbit>> orbits);
        }

        [JsonConverter(typeof(CaseConverter<AtomCase, List<Dictionary<string, Atom>>>))]
        private class AtomCase : ArrayOfMapContainer, ICaseValue<AtomCase, List<Dictionary<string, Atom>>>
        {
            private List<Dictionary<string, Atom>> atom;

            public AtomCase Set(List<Dictionary<string, Atom>> value)
            {
                atom = value;
                return this;
            }
            public List<Dictionary<string, Atom>> Get()
            {
                return atom;
            }

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Atom(atom);
            }

            public override string ToString()
            {
                return atom.ToString();
            }
        }

        [JsonConverter(typeof(CaseConverter<OrbitCase, List<Dictionary<string, Orbit>>>))]
        private class OrbitCase : ArrayOfMapContainer, ICaseValue<OrbitCase, List<Dictionary<string, Orbit>>>
        {
            private List<Dictionary<string, Orbit>> orbit;

            public List<Dictionary<string, Orbit>> Get()
            {
                return orbit;
            }

            public OrbitCase Set(List<Dictionary<string, Orbit>> value)
            {
                orbit = value;
                return this;
            }

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Orbit(orbit);
            }

            public override string ToString()
            {
                return orbit.ToString();
            }
        }

        private class ArrayOfMapConverter : JsonConverter<ArrayOfMapContainer>
        {
            public override ArrayOfMapContainer ReadJson(JsonReader reader, Type objectType, ArrayOfMapContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(AtomCase), "atom" },
                    { typeof(OrbitCase), "orbit" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, false);
                return deserializedObject as ArrayOfMapContainer;
            }

            public override void WriteJson(JsonWriter writer, ArrayOfMapContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
