using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Json.Pointer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities.Json
{
    internal static class JsonPointerAccessor
    {
        public static string ResolveJsonValueByReference(string pointerString, string jsonBody, string jsonHeaders)
        {
            if (string.IsNullOrEmpty(pointerString) || !pointerString.Contains('#'))
                return null;

            var (scope, path) = (pointerString.Split('#')[0], pointerString.Split('#')[1]);
            if (string.IsNullOrEmpty(path))
                return null;

            var jsonPointer = new JsonPointer(path);

            return scope switch
            {
                "$response.body" => GetJsonValueByPointer(jsonPointer, jsonBody),
                "$response.headers" => GetJsonValueByPointer(jsonPointer, jsonHeaders),
                _ => null
            };
        }
        
        private static string GetJsonValueByPointer(JsonPointer jsonPointer, string json)
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

        public static T UpdateBodyValueByPointer<T>(T value, string pointer, Func<object, object> updater)
        {
            if (EqualityComparer<T>.Default.Equals(value, default) || string.IsNullOrEmpty(pointer) || updater == null)
                return value;

            try
            {
                var json = CoreHelper.JsonSerialize(value);
                var jsonObject = JObject.Parse(json);
                var jsonPointer = new JsonPointer(pointer);
                var jsonToken = jsonPointer.Evaluate(jsonObject);

                var oldValue = jsonToken.ToObject<object>();
                var newValue = updater(oldValue);
                if (newValue == null) return value;

                jsonToken.Replace(JToken.FromObject(newValue));
                return CoreHelper.JsonDeserialize<T>(jsonObject.ToString());
            }
            catch { return value; }
        }

        public static Dictionary<string, object> UpdateValueByPointer(Dictionary<string, object> value,
            string pointer, Func<object, object> updater)
        {
            if (value is null || string.IsNullOrEmpty(pointer) || updater == null) return value;
            try
            {
                var typeMap = value.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.GetType() ?? typeof(object));
                var root = JToken.Parse(JsonConvert.SerializeObject(value));
                var jsonPointer = new JsonPointer(pointer);
                var tokenToUpdate = jsonPointer.Evaluate(root);
                var updatedValue = updater(tokenToUpdate.ToObject<object>());
                var newToken = updatedValue == null ? JValue.CreateNull() : JToken.FromObject(updatedValue);

                switch (tokenToUpdate.Parent)
                {
                    case JProperty prop: prop.Value = newToken; break;
                    case JArray array:
                        array[array.IndexOf(tokenToUpdate)] = newToken;
                        break;
                }

                return ((JObject)root).Properties().ToDictionary(prop => prop.Name,
                    prop => prop.Value.ToObject(typeMap.TryGetValue(prop.Name, out var t) ? t : typeof(object)));
            }
            catch { return value; }
        }
    }
}
