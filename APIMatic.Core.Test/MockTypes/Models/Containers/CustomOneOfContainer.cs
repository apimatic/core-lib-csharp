﻿using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;
using System;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<CustomOneOfContainer>),
        new Type[] {
            typeof(AtomCase),
            typeof(OrbitCase)
        },
        true
    )]
    public abstract class CustomOneOfContainer
    {
        public static CustomOneOfContainer FromAtom(Atom value)
        {
            return new AtomCase().Set(value);
        }

        public static CustomOneOfContainer Fromorbit(Orbit value)
        {
            return new OrbitCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(Atom value);

            T Orbit(Orbit value);
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<AtomCase, Atom>))]
        private class AtomCase : CustomOneOfContainer, ICaseValue<AtomCase, Atom>
        {
            public Atom value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Atom(value);
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

        [JsonConverter(typeof(UnionTypeCaseConverter<OrbitCase, Orbit>))]
        private class OrbitCase : CustomOneOfContainer, ICaseValue<OrbitCase, Orbit>
        {
            public Orbit value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Orbit(value);
            }

            public Orbit Get()
            {
                return value;
            }

            public OrbitCase Set(Orbit value)
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
