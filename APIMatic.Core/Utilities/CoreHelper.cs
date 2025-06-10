// <copyright file="CoreHelper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk;
using Microsoft.Json.Pointer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Net;

namespace APIMatic.Core.Utilities
{
    public class CoreHelper
    {
        /// <summary>
        /// DateTime format to use for parsing and converting dates.
        /// </summary>
        internal static readonly string DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
        internal static readonly Dictionary<ArraySerialization, string> _serializationFormats = new Dictionary<ArraySerialization, string>()
        {
            {ArraySerialization.UnIndexed, "{0}[]={{0}}{{1}}" },
            {ArraySerialization.Indexed, "{0}[{{2}}]={{0}}{{1}}"},
            {ArraySerialization.Plain, "{0}={{0}}{{1}}"},
            {ArraySerialization.PSV, "{0}="},
            {ArraySerialization.CSV, "{0}="},
            {ArraySerialization.TSV, "{0}="}
        };

        /// <summary>
        /// JSON Serialization of a given object.
        /// </summary>
        /// <param name="obj">The object to serialize into JSON.</param>
        /// <param name="converter">The converter to use for date time conversion.</param>
        /// <returns>The serialized Json string representation of the given object.</returns>
        public static string JsonSerialize(object obj, JsonConverter converter = null)
        {
            if (obj == null)
            {
                return null;
            }

            var settings = new JsonSerializerSettings()
            {
                MaxDepth = 128
            };

            if (obj.GetType().GetTypeInfo().GetCustomAttribute(typeof(JsogAttribute), false) != null)
            {
                settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            }

            if (converter == null)
            {
                settings.Converters.Add(new IsoDateTimeConverter());
            }
            else
            {
                settings.Converters.Add(converter);
            }

            return JsonConvert.SerializeObject(obj, Formatting.None, settings);
        }

        /// <summary>
        /// JSON Deserialization of the given json string.
        /// </summary>
        /// <param name="json">The json string to deserialize.</param>
        /// <param name="converter">The converter to use for date time conversion.</param>
        /// <typeparam name="T">The type of the object to desialize into.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T JsonDeserialize<T>(string json, JsonConverter converter = null)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            if (converter == null)
            {
                return JsonConvert.DeserializeObject<T>(json, new IsoDateTimeConverter());
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json, converter);
            }
        }

        internal static IDictionary<string, object> ExtractQueryParametersForUrl(string queryUrl)
        { 
            var queryParams = queryUrl.Split('?');

            if (queryParams.Length <= 1)
            {
                return new Dictionary<string, object>();
            }

            return queryParams[1].Split('&').Select(param => param.Split(new []{'='}, 2))
                .ToDictionary(
                    pair => WebUtility.UrlDecode(pair[0]),
                    pair => (object)(pair.Length > 1 ? WebUtility.UrlDecode(pair[1]) : string.Empty)
                );
        }

        /// <summary>
        /// Appends the given set of parameters to the given query string.
        /// </summary>
        /// <param name="queryBuilder">The queryBuilder to append the parameters.</param>
        /// <param name="parameters">The parameters to append.</param>
        /// <param name="arraySerialization">arraySerializationFormat.</param>
        public static void AppendUrlWithQueryParameters(StringBuilder queryBuilder, IEnumerable<KeyValuePair<string, object>> parameters, ArraySerialization arraySerialization = ArraySerialization.UnIndexed)
        {
            if (!parameters.Any())
            {
                return;
            }
            // does the query string already has parameters
            bool hasParams = IndexOf(queryBuilder, "?") > 0;
            var processedParameters = ProcessQueryParamsForCustomTypes(parameters);
            foreach (KeyValuePair<string, object> pair in processedParameters)
            {
                // if already has parameters, use the &amp; to append new parameters
                queryBuilder.Append(hasParams ? '&' : '?');

                // indicate that now the query has some params
                hasParams = true;
                // iterate and append parameters
                AppendParameters(queryBuilder, arraySerialization, pair);
            }
        }

        public static string GetValueByReference(string pointerString, string jsonBody, string jsonHeaders)
        {
            if (pointerString == null)
                return null;

            var parts = pointerString.Split('#');

            if (parts.Length <= 1)
                return null;

            var jsonPointer = new JsonPointer(parts[1]);

            return parts[0] switch
            {
                "$response.body" => GetJsonValueUsingPointer(jsonPointer, jsonBody),
                "$response.headers" => GetJsonValueUsingPointer(jsonPointer, jsonHeaders),
                _ => null
            };
        }

        private static string GetJsonValueUsingPointer(JsonPointer pointer, string json)
        {
            if (pointer == null || json == null)
                return null;

            var jsonObject = JObject.Parse(json);
            JToken jsonToken;
            try
            {
                jsonToken = pointer.Evaluate(jsonObject);
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

        internal static T UpdateValueByPointer<T>(T value, JsonPointer pointer, Func<object, object> updater)
        {
            if (pointer == null || updater == null)
                return value;

            try
            {
                // Step 1: Serialize the object to JSON
                var json = JsonSerialize(value);
                if (string.IsNullOrWhiteSpace(json)) return value;

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

        private static void AppendParameters(StringBuilder queryBuilder, ArraySerialization arraySerialization, KeyValuePair<string, object> pair)
        {
            string paramKeyValPair;

            // load element value as string
            if (pair.Value is ICollection)
            {
                paramKeyValPair = FlattenCollection(pair.Value as ICollection, arraySerialization, Uri.EscapeDataString(pair.Key));
            }
            else
            {
                paramKeyValPair = string.Format("{0}={1}", Uri.EscapeDataString(pair.Key), GetElementValue(pair.Value, true));
            }

            // append keyval pair for current parameter
            queryBuilder.Append(paramKeyValPair);
        }

        /// <summary>
        /// Validates and processes the given query Url to clean empty slashes.
        /// </summary>
        /// <param name="queryBuilder">The given query Url to process.</param>
        /// <returns>Clean Url as string.</returns>
        public static string CleanUrl(StringBuilder queryBuilder)
        {
            // convert to immutable string
            string url = queryBuilder.ToString();

            // ensure that the urls are absolute
            Match match = Regex.Match(url, "^https?://[^/]+");
            if (!match.Success)
            {
                throw new ArgumentException("Invalid Url format.");
            }

            // remove redundant forward slashes
            int index = url.IndexOf('?');
            string protocol = match.Value;
            string query = url.Substring(protocol.Length, (index == -1 ? url.Length : index) - protocol.Length);
            query = Regex.Replace(query, "//+", "/");
            string parameters = index == -1 ? string.Empty : url.Substring(index);

            // return process url
            return string.Concat(protocol, query, parameters);
        }

        internal static bool IsScalarType(Type type)
        {
            return Type.GetTypeCode(type) != TypeCode.Object || type == typeof(VoidType);
        }

        internal static bool IsNullableType(Type type)
        {
            // Check if it's a reference type or a nullable value type
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Prepares parameters for serialization as a form encoded string by flattening complex Types such as Collections and Models to a list of KeyValuePairs, where each value is a string representation of the original Type.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="value">value.</param>
        /// <param name="keys">keys.</param>
        /// <param name="propInfo">propInfo.</param>
        /// <param name="arraySerializationFormat">arraySerializationFormat.</param>
        /// <returns>List of KeyValuePairs.</returns>
        internal static List<KeyValuePair<string, object>> PrepareFormFieldsFromObject(string name, object value, ArraySerialization arraySerializationFormat, List<KeyValuePair<string, object>> keys = null, PropertyInfo propInfo = null)
        {
            keys = keys ?? new List<KeyValuePair<string, object>>();

            if (value == null)
            {
                return keys;
            }

            if (TryGetInnerValueForContainer(value, out dynamic innerValue))
            {
                return PrepareFormFieldsFromObject(name, innerValue, arraySerializationFormat, keys, propInfo);
            }

            if (value is JObject)
            {
                PrepareFormFieldsForJObject(name, value, arraySerializationFormat, keys, propInfo);
            }
            else if (value is IList enumerable)
            {
                PrepareFormFieldsForEnumerable(name, enumerable, arraySerializationFormat, keys, propInfo);
            }
            else if (value is Stream || value is JToken || value is Enum)
            {
                keys.Add(new KeyValuePair<string, object>(name, GetProcessedValue(value)));
            }
            else if (value is IDictionary dictionary)
            {
                PrepareFormFieldsForDictionary(name, dictionary, arraySerializationFormat, keys, propInfo);
            }
            else if (value is CoreJsonObject || value is CoreJsonValue)
            {
                PrepareFormFieldsFromObject(name, GetProcessedValue(value), arraySerializationFormat, keys, propInfo);
            }
            else if (!value.GetType().Namespace.StartsWith("System"))
            {
                PrepareFormFieldsForCustomTypes(name, value, arraySerializationFormat, keys);
            }
            else
            {
                keys.Add(new KeyValuePair<string, object>(name, GetProcessedValue(value, propInfo)));
            }
            return keys;
        }

        internal static bool TryGetInnerValueForContainer(object value, out dynamic innerValue)
        {
            var valueType = value?.GetType();
            if (valueType?.Namespace?.EndsWith(".Containers") == true)
            {
                innerValue = valueType.GetFields()[0].GetValue(value);
                return true;
            }
            innerValue = null;
            return false;
        }

        private static void PrepareFormFieldsForJObject(string name, object value, ArraySerialization arraySerializationFormat, List<KeyValuePair<string, object>> keys, PropertyInfo propInfo)
        {
            var valueAccept = value as JObject;
            foreach (var property in valueAccept.Properties())
            {
                string pKey = property.Name;
                object pValue = property.Value;
                var fullSubName = name + '[' + pKey + ']';
                PrepareFormFieldsFromObject(fullSubName, pValue, arraySerializationFormat, keys, propInfo);
            }
        }

        private static string GetConvertedValue(object value, PropertyInfo propInfo)
        {
            string convertedValue = null;
            object[] pInfo = null;

            if (propInfo != null)
            {
                pInfo = propInfo.GetCustomAttributes(true);
            }

            if (pInfo != null)
            {
                foreach (object attr in pInfo)
                {
                    if (attr is JsonConverterAttribute converterAttr)
                    {
                        convertedValue = JsonSerialize(value, (JsonConverter)Activator.CreateInstance(converterAttr.ConverterType, converterAttr.ConverterParameters)).Replace("\"", string.Empty);
                    }
                }
            }

            return convertedValue;
        }

        private static void PrepareFormFieldsForCustomTypes(string name, object value,
            ArraySerialization arraySerializationFormat, List<KeyValuePair<string, object>> keys)
        {
            // Custom object Iterate through its properties
            var enumerator = value.GetType().GetProperties().GetEnumerator();
            var t = new JsonPropertyAttribute().GetType();
            while (enumerator.MoveNext())
            {
                var pInfo = enumerator.Current as PropertyInfo;
                if (pInfo?.GetIndexParameters().Length != 0) { continue; }

                var jsonProperty = (JsonPropertyAttribute)pInfo.GetCustomAttributes(t, true).FirstOrDefault();

                var subName = (jsonProperty != null) ? jsonProperty.PropertyName : pInfo.Name;
                string fullSubName = string.IsNullOrWhiteSpace(name) ? subName : name + '[' + subName + ']';
                var subValue = pInfo.GetValue(value, null);
                PrepareFormFieldsFromObject(fullSubName, subValue, arraySerializationFormat, keys, pInfo);
            }

            ProcessAdditionalProperties(name, value, arraySerializationFormat, keys);
        }

        private static void ProcessAdditionalProperties(
            string name, object value, ArraySerialization arraySerializationFormat,
            List<KeyValuePair<string, object>> keys)
        {
            // Find a member (field or property) with the [JsonExtensionData] attribute.
            var additionalPropertiesMember = value.GetType()
                .GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(member => member.GetCustomAttribute<JsonExtensionDataAttribute>() != null);

            if (additionalPropertiesMember == null)
            {
                return;
            }

            object additionalProperties = additionalPropertiesMember is FieldInfo fieldInfo
                ? fieldInfo.GetValue(value)
                : (additionalPropertiesMember as PropertyInfo)?.GetValue(value);

            switch (additionalProperties)
            {
                case IDictionary<string, JToken> additionalPropertiesJToken:
                    HandleAdditionalProperties(additionalPropertiesJToken, name, arraySerializationFormat, keys);
                    return;

                case IDictionary<string, object> additionalPropertiesObj:
                    HandleAdditionalProperties(additionalPropertiesObj, name, arraySerializationFormat, keys);
                    return;
            }
        }

        private static void HandleAdditionalProperties<T>(IDictionary<string, T> properties, string name,
            ArraySerialization arraySerializationFormat, List<KeyValuePair<string, object>> keys)
        {
            foreach (var kvp in properties)
            {
                string fullSubName = string.IsNullOrWhiteSpace(name) ? kvp.Key : $"{name}[{kvp.Key}]";
                PrepareFormFieldsFromObject(fullSubName, kvp.Value, arraySerializationFormat, keys, null);
            }
        }

        private static void PrepareFormFieldsForDictionary(string name, IDictionary dictionary,
            ArraySerialization arraySerializationFormat, List<KeyValuePair<string, object>> keys = null,
            PropertyInfo propInfo = null)
        {
            foreach (var sName in dictionary.Keys)
            {
                var subName = sName.ToString();
                var subValue = dictionary[subName];
                string fullSubName = string.IsNullOrWhiteSpace(name) ? subName : name + '[' + subName + ']';
                PrepareFormFieldsFromObject(fullSubName, subValue, arraySerializationFormat, keys, propInfo);
            }
        }

        private static void PrepareFormFieldsForEnumerable(string name, IList enumerable, ArraySerialization arraySerializationFormat, List<KeyValuePair<string, object>> keys, PropertyInfo propInfo)
        {
            var enumerator = enumerable.GetEnumerator();
            bool hasNested = HasCustomeNestedType(enumerator);
            int i = 0;
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                var fullSubName = name + '[' + i + ']';
                if (!hasNested && arraySerializationFormat == ArraySerialization.UnIndexed)
                {
                    fullSubName = name + "[]";
                }
                else if (!hasNested && arraySerializationFormat == ArraySerialization.Plain)
                {
                    fullSubName = name;
                }

                var subValue = enumerator.Current;
                if (subValue == null)
                {
                    continue;
                }
                PrepareFormFieldsFromObject(fullSubName, subValue, arraySerializationFormat, keys, propInfo);
                i++;
            }
        }

        private static bool HasCustomeNestedType(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                var subValue = enumerator.Current;
                if (subValue != null && (subValue is JObject || subValue is IList || subValue is IDictionary || !subValue.GetType().Namespace.StartsWith("System")))
                {
                    return true;
                }
            }
            return false;
        }

        private static object GetProcessedValue(object value, PropertyInfo propInfo = null)
        {
            if (value is Stream)
            {
                return value;
            }
            if (value is Enum)
            {
                return JsonSerialize(value).Trim('\"');
            }
            if (value is JToken)
            {
                return value.ToString();
            }
            if (value is CoreJsonObject jsonObject)
            {
                return RemoveNullValues(jsonObject.GetStoredObject());
            }
            if (value is CoreJsonValue jsonValue)
            {
                return jsonValue.GetStoredObject();
            }
            if (value is DateTime dateTime)
            {
                return GetConvertedValue(dateTime, propInfo) ?? dateTime.ToString(DateTimeFormat);
            }
            return value;
        }

        /// <summary>
        /// Runs asynchronous tasks synchronously and throws the first caught exception.
        /// It also return the result from the given task
        /// </summary>
        /// <param name="t">The task to be run synchronously.</param>
        public static T RunTask<T>(Task<T> t)
        {
            RunVoidTask(t);
            return t.Result;
        }

        /// <summary>
        /// Runs asynchronous tasks synchronously and throws the first caught exception.
        /// </summary>
        /// <param name="t">The task to be run synchronously.</param>
        public static void RunVoidTask(Task t)
        {
            try
            {
                t.Wait();
            }
            catch (AggregateException e)
            {
                throw e.InnerExceptions[0];
            }
        }

        /// <summary>
        /// Creates a deep clone of an object by serializing it into a json string
        /// and then deserializing back into an object.
        /// </summary>
        /// <typeparam name="T">The type of the obj parameter as well as the return object.</typeparam>
        /// <param name="obj">The object to clone.</param>
        /// <returns>Template.</returns>
        internal static T DeepCloneObject<T>(T obj)
        {
            return JsonDeserialize<T>(JsonSerialize(obj));
        }

        /// <summary>
        /// Returns separator according to the arraySerialization.
        /// </summary>
        /// <param name="arraySerialization">The array serialization format.</param>
        /// <returns>The separator character.</returns>
        private static char GetSeparator(ArraySerialization arraySerialization)
        {
            if (arraySerialization == ArraySerialization.CSV)
            {
                return ',';
            }
            if (arraySerialization == ArraySerialization.PSV)
            {
                return '|';
            }
            if (arraySerialization == ArraySerialization.TSV)
            {
                return '\t';
            }
            return '&';
        }

        /// <summary>
        /// StringBuilder extension method to implement IndexOf functionality.
        /// This does a StringComparison.Ordinal kind of comparison.
        /// </summary>
        /// <param name="stringBuilder">The string builder to find the index in.</param>
        /// <param name="strCheck">The string to locate in the string builder.</param>
        /// <returns>The index of string inside the string builder.</returns>
        private static int IndexOf(StringBuilder stringBuilder, string strCheck)
        {
            // iterate over the input
            for (int inputCounter = 0; inputCounter < stringBuilder.Length; inputCounter++)
            {
                int matchCounter = 0;
                bool IsInBoundOfString() => matchCounter < strCheck.Length;
                bool IsInBoundOfStringBuilder() => inputCounter + matchCounter < stringBuilder.Length;
                bool IsMatchLocated() => stringBuilder[inputCounter + matchCounter] == strCheck[matchCounter];

                while (IsInBoundOfString() && IsInBoundOfStringBuilder() && IsMatchLocated())
                {
                    // attempts to locate a potential match
                    matchCounter++;
                }

                // verify the match
                if (matchCounter == strCheck.Length)
                {
                    return inputCounter;
                }
            }

            return -1;
        }

        /// <summary>
        /// Used for flattening a collection of objects into a string.
        /// </summary>
        /// <param name="array">Array of elements to flatten.</param>
        /// <param name="fmt">Format string to use for array flattening.</param>
        /// <param name="key">Separator to use for string concat.</param>
        /// <returns>Representative string made up of array elements.</returns>
        private static string FlattenCollection(
            ICollection array,
            ArraySerialization fmt,
            string key = "")
        {
            StringBuilder builder = new StringBuilder();
            string format = GetFormatString(fmt, key, builder);

            // append all elements in the array into a string
            int index = 0;
            char separator = GetSeparator(fmt);
            foreach (object element in array)
            {
                builder.AppendFormat(format, GetElementValue(element, true), separator, index++);
            }

            // remove the last separator, if appended
            if ((builder.Length > 1) && (builder[builder.Length - 1] == separator))
            {
                builder.Length -= 1;
            }

            return builder.ToString();
        }

        private static string GetFormatString(ArraySerialization fmt, string key, StringBuilder builder)
        {
            if (_serializationFormats.TryGetValue(fmt, out var format))
            {
                if (fmt == ArraySerialization.CSV || fmt == ArraySerialization.PSV || fmt == ArraySerialization.TSV)
                {
                    builder.Append(string.Format(format, key));
                    format = "{0}{1}";
                }
                else
                {
                    format = string.Format(format, key);
                }
            }

            return format;
        }

        private static string GetElementValue(object element, bool urlEncode)
        {
            if (element is DateTime time)
            {
                return time.ToString(DateTimeFormat);
            }

            if (element is DateTimeOffset offset)
            {
                return offset.ToString(DateTimeFormat);
            }

            string elemValue = element.ToString();

            if (element is bool)
            {
                elemValue = elemValue.ToLower();
            }

            if (urlEncode)
            {
                elemValue = Uri.EscapeDataString(elemValue);
            }

            return elemValue;
        }

        /// <summary>
        /// Apply appropriate serialization to query parameters.
        /// </summary>
        /// <param name="parameters"> Parameters. </param>
        /// <returns> List of processed query parameters. </returns>
        private static List<KeyValuePair<string, object>> ProcessQueryParamsForCustomTypes(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var processedParameters = new List<KeyValuePair<string, object>>();

            foreach (var kvp in parameters)
            {
                // ignore null values
                if (kvp.Value == null)
                {
                    continue;
                }

                if (kvp.Value.GetType().Namespace.StartsWith("System"))
                {
                    HandlePrimitiveTypes(processedParameters, kvp);
                }
                else
                {
                    // Custom type
                    HandleCustomType(processedParameters, kvp);
                }
            }

            return processedParameters;
        }

        private static void HandleCustomType(List<KeyValuePair<string, object>> processedParameters, KeyValuePair<string, object> kvp)
        {
            var list = PrepareFormFieldsFromObject(kvp.Key, kvp.Value, arraySerializationFormat: ArraySerialization.Indexed);
            list = ApplySerializationFormatToScalarArrays(list);
            processedParameters.AddRange(list);
        }

        private static void HandlePrimitiveTypes(List<KeyValuePair<string, object>> processedParameters, KeyValuePair<string, object> kvp)
        {
            if (kvp.Value is IList list)
            {
                HandleListParameter(processedParameters, kvp, list);
                return;
            }
            if (kvp.Value is IDictionary dictionary)
            {
                HandleDictionaryParameter(processedParameters, kvp, dictionary);
                return;
            }
            // Scalar type
            processedParameters.Add(kvp);
        }

        private static void HandleListParameter(List<KeyValuePair<string, object>> processedParameters, KeyValuePair<string, object> kvp, IList list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            HandleParameter(processedParameters, kvp, list[0]);
        }

        private static void HandleDictionaryParameter(List<KeyValuePair<string, object>> processedParameters, KeyValuePair<string, object> kvp, IDictionary dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                return;
            }
            var enumerator = dictionary.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var item = ((DictionaryEntry)enumerator.Current).Value;
                HandleParameter(processedParameters, kvp, item);
            }
        }

        private static void HandleParameter(List<KeyValuePair<string, object>> processedParameters, KeyValuePair<string, object> kvp, object entry)
        {
            if (entry.GetType().Namespace.StartsWith("System"))
            {
                // List of scalar type
                processedParameters.Add(kvp);
                return;
            }
            HandleCustomType(processedParameters, kvp);
        }

        /// <summary>
        /// Apply serialization to scalar arrays in custom objects.
        /// </summary>
        /// <param name="parameters"> Parameters. </param>
        /// <returns> List of processed query parameters. </returns>
        private static List<KeyValuePair<string, object>> ApplySerializationFormatToScalarArrays(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var processedParams = new List<KeyValuePair<string, object>>();
            var unprocessedParams = parameters.Where(x => !IsScalarValuesArray(x.Key));

            // Extract scalar arrays and group them by key
            var arraysGroupedByKey = parameters
                .Where(x => IsScalarValuesArray(x.Key))
                .Select(x =>
                {
                    return new KeyValuePair<string, object>(
                        x.Key.Substring(0, x.Key.LastIndexOf('[')),
                        x.Value);
                })
                .GroupBy(x => x.Key);

            foreach (var group in arraysGroupedByKey)
            {
                var key = group.Key;
                var values = new List<object>();
                foreach (var aaa in group)
                {
                    values.Add(aaa.Value);
                }

                processedParams.Add(new KeyValuePair<string, object>(key, values));
            }

            processedParams.AddRange(unprocessedParams);

            return processedParams;
        }

        /// <summary>
        /// Checks if the provided string is part of a scalar array
        /// </summary>
        /// <param name="input"> Input string.</param>
        /// <returns> True or False </returns>
        private static bool IsScalarValuesArray(string input)
        {
            var regex = new Regex("\\[\\d+\\]$", RegexOptions.IgnoreCase);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// Removes null values for fields preparation for forms
        /// </summary>
        /// <param name="token"> Input from which null values have to be removed.</param>
        /// <returns> JToken without null values </returns>
        private static JToken RemoveNullValues(JToken token)
        {
            JObject copy = new JObject();

            foreach (JProperty prop in token.Children<JProperty>())
            {
                JToken child = prop.Value;
                if (child.HasValues)
                {
                    child = RemoveNullValues(child);
                }
                if (child.Type != JTokenType.Null)
                {
                    copy.Add(prop.Name, child);
                }
            }

            return copy;
        }
    }
}
