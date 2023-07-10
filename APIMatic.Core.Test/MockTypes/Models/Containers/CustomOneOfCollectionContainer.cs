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
            return new AtomCase(atoms);
        }

        public static CustomOneOfCollectionContainer Fromorbit(Orbit[] orbits)
        {
            return new OrbitCase(orbits);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<T>
        {
            T Atom(Atom[] atoms);

            T Orbit(Orbit[] orbits);
        }

        [JsonConverter(typeof(AtomCaseConverter))]
        private class AtomCase : CustomOneOfCollectionContainer
        {
            [JsonProperty]
            internal readonly Atom[] atom;

            public AtomCase(Atom[] atom)
            {
                this.atom = atom;
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

        [JsonConverter(typeof(OrbitCaseConverter))]
        private class OrbitCase : CustomOneOfCollectionContainer
        {
            [JsonProperty]
            internal readonly Orbit[] orbit;

            public OrbitCase(Orbit[] orbit)
            {
                this.orbit = orbit;
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

        private class AtomCaseConverter : JsonConverter<AtomCase>
        {
            public override AtomCase ReadJson(JsonReader reader, Type objectType, AtomCase existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                try
                {
                    Atom[] value = serializer.Deserialize<Atom[]>(reader);
                    return new AtomCase(value);
                }
                catch (Exception)
                {
                    throw new JsonSerializationException("Invalid AtomCase");
                }
            }

            public override void WriteJson(JsonWriter writer, AtomCase value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.atom);
            }
        }


        private class OrbitCaseConverter : JsonConverter<OrbitCase>
        {
            public override OrbitCase ReadJson(JsonReader reader, Type objectType, OrbitCase existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                try
                {
                    Orbit[] value = serializer.Deserialize<Orbit[]>(reader);
                    return new OrbitCase(value);
                }
                catch (Exception)
                {
                    throw new JsonSerializationException("Invalid OrbitCase");
                }
            }

            public override void WriteJson(JsonWriter writer, OrbitCase value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.orbit);
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
