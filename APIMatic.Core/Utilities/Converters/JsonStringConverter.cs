using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities.Converters
{
    public class JsonStringConverter : JsonConverter
    {
        private readonly bool _required;

        public JsonStringConverter()
        {
            _required = false;
        }

        public JsonStringConverter(bool required)
        {
            _required = required;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (CanConvert(objectType) && reader.TokenType == JsonToken.String)
            {
                return reader.Value;
            }
            if (_required)
            {
                throw new JsonSerializationException("Invalid string.");
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value);

            if (token.Type == JTokenType.String)
            {
                JValue val = (JValue)token;

                val.WriteTo(writer);
            }
        }
    }

}
