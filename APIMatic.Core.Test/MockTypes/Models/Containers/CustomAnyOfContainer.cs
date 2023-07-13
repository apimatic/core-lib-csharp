using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(CustomAnyOfConverter))]
    public abstract class CustomAnyOfContainer
    {
        public static CustomAnyOfContainer FromAtom(Atom value)
        {
            return new AtomCase().Set(value);
        }

        public static CustomAnyOfContainer Fromorbit(Orbit value)
        {
            return new OrbitCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(Atom value);

            T Orbit(Orbit value);
        }

        [JsonConverter(typeof(CaseConverter<AtomCase, Atom>))]
        private class AtomCase : CustomAnyOfContainer, ICaseValue<AtomCase, Atom>
        {
            private Atom value;

            public AtomCase Set(Atom value)
            {
                this.value = value;
                return this;
            }
            public Atom Get()
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

        [JsonConverter(typeof(CaseConverter<OrbitCase, Orbit>))]
        private class OrbitCase : CustomAnyOfContainer, ICaseValue<OrbitCase, Orbit>
        {
            private Orbit value;

            public Orbit Get()
            {
                return value;
            }

            public OrbitCase Set(Orbit value)
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
