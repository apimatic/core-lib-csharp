using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{

    [JsonConverter(typeof(NativeOneOfCollectionConverter))]
    public abstract class NativeOneOfCollectionContainer
    {
        public static NativeOneOfCollectionContainer FromPrecisionArray(double[] precision)
        {
            return new PrecisionArrayCase(precision);
        }

        public static NativeOneOfCollectionContainer FromMString(string[] mString)
        {
            return mString == null || mString.Length == 0 ? null : new MStringArrayCase(mString);
        }

        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<T>
        {
            T Precision(double[] precision);

            T MString(string[] mString);
        }

        [JsonConverter(typeof(PrecisionArrayCaseConverter))]
        private class PrecisionArrayCase : NativeOneOfCollectionContainer
        {
            [JsonProperty]
            internal readonly double[] precision;

            public PrecisionArrayCase(double[] precision)
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

        [JsonConverter(typeof(StringArrayCaseConverter))]
        private class MStringArrayCase : NativeOneOfCollectionContainer
        {
            [JsonProperty]
            internal readonly string[] mString;

            public MStringArrayCase(string[] mString)
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

        private class PrecisionArrayCaseConverter : JsonConverter<PrecisionArrayCase>
        {
            public override PrecisionArrayCase ReadJson(JsonReader reader, Type objectType, PrecisionArrayCase existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                try
                {
                    double[] doubles = serializer.Deserialize<double[]>(reader);
                    return new PrecisionArrayCase(doubles);
                }
                catch (Exception)
                {
                    throw new JsonSerializationException("Invalid PrecisionCase");
                }
            }

            public override void WriteJson(JsonWriter writer, PrecisionArrayCase value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.precision);
            }
        }


        private class StringArrayCaseConverter : JsonConverter<MStringArrayCase>
        {
            public override MStringArrayCase ReadJson(JsonReader reader, Type objectType, MStringArrayCase existingValue, bool hasExistingValue, JsonSerializer serializer)
            {

                if (reader.TokenType == JsonToken.StartArray)
                {
                    var stringArray = new List<string>();

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.String)
                        {
                            string value = (string)reader.Value;
                            stringArray.Add(value);
                        }
                        else if (reader.TokenType == JsonToken.EndArray)
                        {
                            break;
                        }
                        else
                        {
                            throw new JsonSerializationException("Invalid StringArrayCase");
                        }
                    }

                    return new MStringArrayCase(stringArray.ToArray());
                }
                else
                {
                    throw new JsonSerializationException("Invalid StringArrayCase");
                }
            }

            public override void WriteJson(JsonWriter writer, MStringArrayCase value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.mString);
            }
        }

        private class NativeOneOfCollectionConverter : JsonConverter<NativeOneOfCollectionContainer>
        {
            public override bool CanRead => true;
            public override bool CanWrite => false;

            public override NativeOneOfCollectionContainer ReadJson(JsonReader reader, Type objectType, NativeOneOfCollectionContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                Dictionary<Type, string> types = new Dictionary<Type, string>
                {
                    { typeof(PrecisionArrayCase), "precision" },
                    { typeof(MStringArrayCase), "string" }
                };
                var deserializedObject = CoreHelper.TryDeserializeOneOfAnyOf(types, reader, serializer, true);
                return deserializedObject as NativeOneOfCollectionContainer;
            }

            public override void WriteJson(JsonWriter writer, NativeOneOfCollectionContainer value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
