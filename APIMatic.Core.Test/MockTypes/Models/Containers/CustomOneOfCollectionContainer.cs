using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(CustomOneOfCollectionConverter))]
    public abstract class CustomOneOfCollectionContainer
    {
        public static CustomOneOfCollectionContainer FromAtom(Atom[] atoms)
        {
            return new AtomCase().Set(atoms);
        }

        public static CustomOneOfCollectionContainer Fromorbit(Orbit[] orbits)
        {
            return new OrbitCase().Set(orbits);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(Atom[] atoms);

            T Orbit(Orbit[] orbits);
        }

        [JsonConverter(typeof(CaseConverter<AtomCase, Atom[]>))]
        private class AtomCase : CustomOneOfCollectionContainer, ICaseValue<AtomCase, Atom[]>
        {
            private Atom[] atoms;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Atom(atoms);
            }

            public AtomCase Set(Atom[] value)
            {
                atoms = value;
                return this;
            }
            public Atom[] Get()
            {
                return atoms;
            }

            public override string ToString()
            {
                return atoms.ToString();
            }
        }

        [JsonConverter(typeof(CaseConverter<OrbitCase, Orbit[]>))]
        private class OrbitCase : CustomOneOfCollectionContainer, ICaseValue<OrbitCase, Orbit[]>
        {
            private Orbit[] orbits;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Orbit(orbits);
            }

            public Orbit[] Get()
            {
                return orbits;
            }

            public OrbitCase Set(Orbit[] value)
            {
                orbits = value;
                return this;
            }

            public override string ToString()
            {
                return orbits.ToString();
            }
        }

        private class CustomOneOfCollectionConverter : JsonConverter<CustomOneOfCollectionContainer>
        {
            public override CustomOneOfCollectionContainer ReadJson(JsonReader reader, Type objectType, CustomOneOfCollectionContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(AtomCase), "atom" },
                    { typeof(OrbitCase), "orbit" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, true);
                return deserializedObject as CustomOneOfCollectionContainer;
            }

            public override void WriteJson(JsonWriter writer, CustomOneOfCollectionContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
