using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(typeof(NativeConverter))]
    public abstract class NativeAnyOfContainer
    {
        public static NativeAnyOfContainer FromPrecision(double precision)
        {
            return new PrecisionCase(precision);
        }

        public static NativeAnyOfContainer FromMString(string mString)
        {
            return string.IsNullOrEmpty(mString) ? null : new MStringCase(mString);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<T>
        {
            T Precision(double precision);

            T MString(string mString);
        }

        [JsonConverter(typeof(PrecisionCaseConverter))]
        private class PrecisionCase : NativeAnyOfContainer
        {
            [JsonProperty]
            internal readonly double precision;

            public PrecisionCase(double precision)
            {
                this.precision = precision;
            }

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Precision(precision);
            }

            public override string ToString()
            {
                return precision.ToString();
            }
        }

        [JsonConverter(typeof(StringCaseConverter))]
        private class MStringCase : NativeAnyOfContainer
        {
            [JsonProperty]
            internal readonly string mString;

            public MStringCase(string mString)
            {
                this.mString = mString;
            }

            public override T Match<T>(ICases<T> cases)
            {
                return cases.MString(mString);
            }

            public override string ToString()
            {
                return mString.ToString();
            }
        }

        private class PrecisionCaseConverter : JsonConverter<PrecisionCase>
        {
            public override PrecisionCase ReadJson(JsonReader reader, Type objectType, PrecisionCase existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.Float)
                {
                    throw new JsonSerializationException("Invalid PrecisionCase");
                }
                try
                {
                    double value = serializer.Deserialize<double>(reader);
                    return new PrecisionCase(value);
                }
                catch (Exception)
                {
                    throw new JsonSerializationException("Invalid PrecisionCase");
                }
            }

            public override void WriteJson(JsonWriter writer, PrecisionCase value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.precision);
            }
        }


        private class StringCaseConverter : JsonConverter<MStringCase>
        {
            public override MStringCase ReadJson(JsonReader reader, Type objectType, MStringCase existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.String)
                {
                    throw new JsonSerializationException("Invalid StringCase");
                }

                try
                {
                    string value = serializer.Deserialize<string>(reader);
                    return new MStringCase(value);
                }
                catch (Exception)
                {
                    throw new JsonSerializationException("Invalid StringCase");
                }
            }

            public override void WriteJson(JsonWriter writer, MStringCase value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.mString);
            }
        }

        private class NativeConverter : JsonConverter<NativeAnyOfContainer>
        {
            public override bool CanRead => true;
            public override bool CanWrite => false;

            public override NativeAnyOfContainer ReadJson(JsonReader reader, Type objectType, NativeAnyOfContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(PrecisionCase), "precision" },
                    { typeof(MStringCase), "string" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, false);
                return deserializedObject as NativeAnyOfContainer;
            }

            public override void WriteJson(JsonWriter writer, NativeAnyOfContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
