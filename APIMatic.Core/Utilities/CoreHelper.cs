using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIMatic.Core.Utilities
{
    class CoreHelper
    {
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
    }
}
