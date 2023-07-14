﻿using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
       typeof(UnionTypeConverter<MapOfArrayContainer>),
       new Type[] {
            typeof(AtomCase),
            typeof(OrbitCase)
       },
       false
   )]
    public abstract class MapOfArrayContainer
    {
        public static MapOfArrayContainer FromAtom(Dictionary<string, List<Atom>> value)
        {
            return new AtomCase().Set(value);
        }

        public static MapOfArrayContainer Fromorbit(Dictionary<string, List<Orbit>> value)
        {
            return new OrbitCase().Set(value);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T Atom(Dictionary<string, List<Atom>> value);

            T Orbit(Dictionary<string, List<Orbit>> value);
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<AtomCase, Dictionary<string, List<Atom>>>))]
        private class AtomCase : MapOfArrayContainer, ICaseValue<AtomCase, Dictionary<string, List<Atom>>>
        {
            private Dictionary<string, List<Atom>> value;

            public AtomCase Set(Dictionary<string, List<Atom>> value)
            {
                this.value = value;
                return this;
            }
            public Dictionary<string, List<Atom>> Get()
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

        [JsonConverter(typeof(UnionTypeCaseConverter<OrbitCase, Dictionary<string, List<Orbit>>>))]
        private class OrbitCase : MapOfArrayContainer, ICaseValue<OrbitCase, Dictionary<string, List<Orbit>>>
        {
            private Dictionary<string, List<Orbit>> value;

            public Dictionary<string, List<Orbit>> Get()
            {
                return value;
            }

            public OrbitCase Set(Dictionary<string, List<Orbit>> value)
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
