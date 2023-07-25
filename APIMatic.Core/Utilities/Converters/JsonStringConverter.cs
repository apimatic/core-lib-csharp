using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace APIMatic.Core.Utilities.Converters
{
    public class JsonStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            return token.Type == JTokenType.String ? token.ToObject<string>() : throw new JsonSerializationException("Invalid string.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);
    }

}
