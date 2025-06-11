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

        public static T UpdateValueByPointer<T>(T value, JsonPointer pointer, Func<object, object> updater)
        {
            if (value == null || pointer == null || updater == null)
                return value;

            try
            {
                // Step 1: Serialize the object to JSON
                var json = CoreHelper.JsonSerialize(value);

                // Step 2: Parse JSON and evaluate pointer
                var jsonObject = JObject.Parse(json);
                var jsonToken = pointer.Evaluate(jsonObject);

                // Step 3: Apply updater
                var oldValue = jsonToken.ToObject<object>();
                var newValue = updater(oldValue);
                if (newValue == null) return value;

                // Step 4: Replace token
                jsonToken.Replace(JToken.FromObject(newValue));

                // Step 5: Deserialize back to T
                var updatedJson = jsonObject.ToString();
                return CoreHelper.JsonDeserialize<T>(updatedJson);
            }
            catch
            {
                return value;
            }
        }

        public static Dictionary<string, object> UpdateValueByPointer(
            Dictionary<string, object> value,
            JsonPointer pointer,
            Func<object, object> updater)
        {
            try
            {
                // 0. Capture original types of root dictionary values
                var typeMap = value.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.GetType() ?? typeof(object));

                // 1. Serialize the input dictionary to JToken
                var json = JsonConvert.SerializeObject(value);
                var root = JToken.Parse(json);

                // 2. Locate the token to update
                var tokenToUpdate = pointer.Evaluate(root);

                // 3. Apply updater
                var currentValue = tokenToUpdate.ToObject<object>();
                var updatedValue = updater(currentValue);
                var newToken = updatedValue == null ? JValue.CreateNull() : JToken.FromObject(updatedValue);

                switch (tokenToUpdate.Parent)
                {
                    // 4. Replace token
                    case JProperty prop:
                        prop.Value = newToken;
                        break;
                    case JArray array:
                    {
                        int index = array.IndexOf(tokenToUpdate);
                        array[index] = newToken;
                        break;
                    }
                }

                // 5. Deserialize each value using its original type
                var result = new Dictionary<string, object>();
                foreach (var kvp in ((JObject)root).Properties())
                {
                    var key = kvp.Name;
                    var token = kvp.Value;
                    var targetType = typeMap.TryGetValue(key, out var type) ? type : typeof(object);
                    var obj = token.ToObject(targetType);
                    result[key] = obj!;
                }

                return result;
            }
            catch
            {
                return value;
            }
        }
    }
}
