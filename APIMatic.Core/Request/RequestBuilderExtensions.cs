using System;
using System.Collections.Generic;
using System.Linq;
using APIMatic.Core.Utilities;
using Microsoft.Json.Pointer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Request
{
    internal static class RequestBuilderExtensions
    {
        public static Dictionary<string, string> ToCoreRequestHeaders(this Dictionary<string, object> headers)
        {
            var result = new Dictionary<string, string>();

            foreach (var kvp in headers)
            {
                var serialized = CoreHelper.JsonSerialize(kvp.Value)?.TrimStart('"').TrimEnd('"');
                if (serialized != null)
                {
                    result[kvp.Key] = serialized;
                }
            }

            return result;
        }

        public static void UpdateRequestParametersByPointer(Dictionary<string, object> requestParameters,
            string pointer,
            Func<object, object> setter)
        {
            var updated =
                UpdateValueByPointer(requestParameters, pointer, setter);

            foreach (var kvp in updated)
            {
                if (!requestParameters.TryGetValue(kvp.Key, out var existingValue) || !Equals(existingValue, kvp.Value))
                {
                    requestParameters[kvp.Key] = kvp.Value;
                }
            }
        }

        public static void UpdateFormParameterValueByPointer(List<KeyValuePair<string, object>> formParameters,
            string pointer,
            Func<object, object> setter)
        {
            var formParameterDictionary = formParameters
                .GroupBy(p => p.Key)
                .ToDictionary(g => g.Key, g => g.Last().Value); // Handles duplicate keys safely
            
            var updated =
                UpdateValueByPointer(formParameterDictionary, pointer, setter);
            
            formParameters.Clear();
            formParameters.AddRange(updated.Select(kvp => new KeyValuePair<string, object>(kvp.Key, kvp.Value)));
        }

        public static T UpdateValueByPointer<T>(T value, string pointer, Func<object, object> updater)
        {
            if (value == null || string.IsNullOrEmpty(pointer) || updater == null)
                return value;

            try
            {
                var type = value.GetType();
                var rootToken = JToken.Parse(JsonConvert.SerializeObject(value));
                var tokenToUpdate = new JsonPointer(pointer).Evaluate(rootToken);

                var updatedValue = updater(tokenToUpdate.ToObject<object>());
                if (updatedValue == null) return value;

                ReplaceToken(tokenToUpdate, updatedValue);

                return typeof(T) == typeof(Dictionary<string, object>)
                    ? (T)(object)ReconstructDictionary((Dictionary<string, object>)(object)value, (JObject)rootToken)
                    : (T)JsonConvert.DeserializeObject(rootToken.ToString(), type);
            }
            catch
            {
                return value;
            }
        }

        private static void ReplaceToken(JToken token, object newValue)
        {
            var newToken = JToken.FromObject(newValue);
            switch (token.Parent)
            {
                case JProperty prop:
                    prop.Value = newToken;
                    break;
                case JArray array:
                    array[array.IndexOf(token)] = newToken;
                    break;
            }
        }

        private static Dictionary<string, object> ReconstructDictionary(
            Dictionary<string, object> original,
            JObject updatedRoot)
        {
            var typeMap = original.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.GetType() ?? typeof(object));
            return updatedRoot.Properties().ToDictionary(
                prop => prop.Name,
                prop => prop.Value.ToObject(typeMap.TryGetValue(prop.Name, out var t) ? t : typeof(object))
            );
        }
    }
}

