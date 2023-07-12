using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(CustomOneOfConverter))]
    public abstract class CustomOneOfContainer
    {
        public static CustomOneOfContainer FromAtom(Atom atom)
        {
            return new AtomCase().Set(atom);
        }

        public static CustomOneOfContainer Fromorbit(Orbit orbit)
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
        private class AtomCase : CustomOneOfContainer, ICaseValue<AtomCase, Atom>
        {
            private Atom atom;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Atom(atom);
            }

            public AtomCase Set(Atom value)
            {
                atom = value;
                return this;
            }

            public Atom Get()
            {
                return atom;
            }

            public override string ToString()
            {
                return atom.ToString();
            }
        }

        [JsonConverter(typeof(CaseConverter<OrbitCase, Orbit>))]
        private class OrbitCase : CustomOneOfContainer, ICaseValue<OrbitCase, Orbit>
        {
            private Orbit orbit;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Orbit(orbit);
            }

            public Orbit Get()
            {
                return orbit;
            }

            public OrbitCase Set(Orbit value)
            {
                orbit = value;
                return this;
            }

            public override string ToString()
            {
                return orbit.ToString();
            }
        }

        private class CustomOneOfConverter : JsonConverter<CustomOneOfContainer>
        {
            public override CustomOneOfContainer ReadJson(JsonReader reader, Type objectType, CustomOneOfContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(AtomCase), "atom" },
                    { typeof(OrbitCase), "orbit" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, true);
                return deserializedObject as CustomOneOfContainer;
            }

            public override void WriteJson(JsonWriter writer, CustomOneOfContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
