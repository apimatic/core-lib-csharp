using System.Linq;
using Microsoft.Json.Pointer;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities.Json
{
    internal static class JsonPointerResolver
    {
        public static string ResolveScopedJsonValue(string pointerString, string jsonBody, string jsonHeaders)
        {
            if (string.IsNullOrEmpty(pointerString) || !pointerString.Contains('#'))
                return null;

            var (scope, path) = (pointerString.Split('#')[0], pointerString.Split('#')[1]);
            if (string.IsNullOrEmpty(path))
                return null;

            var jsonPointer = new JsonPointer(path);

            switch (scope)
            {
                case "$response.body":
                    return ExtractValueByPointer(jsonPointer, jsonBody);
                case "$response.headers":
                    return ExtractValueByPointer(jsonPointer, jsonHeaders);
                default:
                    return null;
            }
        }
        
        private static string ExtractValueByPointer(JsonPointer jsonPointer, string json)
        {
            if (json == null) return null;

            var jsonObject = JObject.Parse(json);
            JToken jsonToken;
            try
            {
                jsonToken = jsonPointer.Evaluate(jsonObject);
            }
            catch
            {
                return null;
            }

            switch (jsonToken.Type)
            {
                case JTokenType.Null:
                    return null;
                case JTokenType.String:
                    return jsonToken.Value<string>();
                default:
                    return jsonToken.ToString();
            }
        }
    }
}
