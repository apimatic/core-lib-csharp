using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Test.MockTypes.Models
{
    public class SimpleModelWithAdditionalPropertiesField
    {
        [JsonExtensionData]
        private readonly IDictionary<string, JToken> _additionalProperties;

        /// <summary>
        /// Set the value associated with the specified key in the AdditionalProperties dictionary.
        /// </summary>
        public string this[string key]
        {
            set => _additionalProperties.SetValue(key, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModelWithAdditionalPropertiesBaseModel"/> class.
        /// </summary>
        /// <param name="requiredProperty">requiredProperty.</param>
        public SimpleModelWithAdditionalPropertiesField(
            string requiredProperty)
        {
            this._additionalProperties = new Dictionary<string, JToken>();
            this.RequiredProperty = requiredProperty;
        }

        /// <summary>
        /// The required property
        /// </summary>
        [JsonProperty("requiredProperty")]
        public string RequiredProperty { get; set; }
    }

    internal static class AdditionalPropertiesExtensions
    {
        internal static void SetValue(this IDictionary<string, JToken> additionalProperties, string key, object value)
        {
            additionalProperties[key] = JToken.FromObject(value);
        }
    }
}
