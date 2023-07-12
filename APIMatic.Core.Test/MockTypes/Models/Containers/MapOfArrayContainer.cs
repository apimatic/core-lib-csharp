using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(MapOfArrayConverter))]
    public abstract class MapOfArrayContainer
    {
        public static MapOfArrayContainer FromAtom(Dictionary<string, List<Atom>> atoms)
        {
            return new AtomCase().Set(atoms);
        }

        public static MapOfArrayContainer Fromorbit(Dictionary<string, List<Orbit>> orbits)
        {
            return new OrbitCase().Set(orbits);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(Dictionary<string, List<Atom>> atoms);

            T Orbit(Dictionary<string, List<Orbit>> orbits);
        }

        [JsonConverter(typeof(CaseConverter<AtomCase, Dictionary<string, List<Atom>>>))]
        private class AtomCase : MapOfArrayContainer, ICaseValue<AtomCase, Dictionary<string, List<Atom>>>
        {
            private Dictionary<string, List<Atom>> atom;

            public AtomCase Set(Dictionary<string, List<Atom>> value)
            {
                atom = value;
                return this;
            }
            public Dictionary<string, List<Atom>> Get()
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

        [JsonConverter(typeof(CaseConverter<OrbitCase, Dictionary<string, List<Orbit>>>))]
        private class OrbitCase : MapOfArrayContainer, ICaseValue<OrbitCase, Dictionary<string, List<Orbit>>>
        {
            private Dictionary<string, List<Orbit>> orbit;

            public Dictionary<string, List<Orbit>> Get()
            {
                return orbit;
            }

            public OrbitCase Set(Dictionary<string, List<Orbit>> value)
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

        private class MapOfArrayConverter : JsonConverter<MapOfArrayContainer>
        {
            public override MapOfArrayContainer ReadJson(JsonReader reader, Type objectType, MapOfArrayContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(AtomCase), "atom" },
                    { typeof(OrbitCase), "orbit" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, false);
                return deserializedObject as MapOfArrayContainer;
            }

            public override void WriteJson(JsonWriter writer, MapOfArrayContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
