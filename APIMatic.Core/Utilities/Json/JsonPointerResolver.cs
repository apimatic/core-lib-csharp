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

            return scope switch
            {
                "$response.body" => ExtractValueByPointer(jsonPointer, jsonBody),
                "$response.headers" => ExtractValueByPointer(jsonPointer, jsonHeaders),
                _ => null
            };
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

            return jsonToken.Type switch
            {
                JTokenType.Null => null,
                JTokenType.String => jsonToken.Value<string>(),
                _ => jsonToken.ToString()
            };
        }
    }
}
