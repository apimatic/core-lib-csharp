using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(CustomAnyOfConverter))]
    public abstract class CustomAnyOfContainer
    {
        public static CustomAnyOfContainer FromAtom(Atom atom)
        {
            return new AtomCase().Set(atom);
        }

        public static CustomAnyOfContainer Fromorbit(Orbit orbit)
        {
            return new OrbitCase().Set(orbit);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(Atom atom);

            T Orbit(Orbit orbit);
        }

        [JsonConverter(typeof(CaseConverter<AtomCase, Atom>))]
        private class AtomCase : CustomAnyOfContainer, ICaseValue<AtomCase, Atom>
        {
            private Atom atom;

            public AtomCase Set(Atom value)
            {
                atom = value;
                return this;
            }
            public Atom Get()
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

        [JsonConverter(typeof(CaseConverter<OrbitCase, Orbit>))]
        private class OrbitCase : CustomAnyOfContainer, ICaseValue<OrbitCase, Orbit>
        {
            private Orbit orbit;

            public Orbit Get()
            {
                return orbit;
            }

            public OrbitCase Set(Orbit value)
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

        private class CustomAnyOfConverter : JsonConverter<CustomAnyOfContainer>
        {
            public override CustomAnyOfContainer ReadJson(JsonReader reader, Type objectType, CustomAnyOfContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(AtomCase), "atom" },
                    { typeof(OrbitCase), "orbit" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, false);
                return deserializedObject as CustomAnyOfContainer;
            }

            public override void WriteJson(JsonWriter writer, CustomAnyOfContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
