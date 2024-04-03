using System;
using Newtonsoft.Json;

namespace APIMatic.Core.Utilities.Converters.Interfaces
{
    public interface IJsonConverter
    {
        bool CanConvert(Type objectType);
        object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);
        void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);
    }
}
