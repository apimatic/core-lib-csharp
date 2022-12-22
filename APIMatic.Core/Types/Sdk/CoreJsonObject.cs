// <copyright file="CoreJsonObject.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Types.Sdk
{
    /// <summary>
    /// This is the base class for JsonObject
    /// </summary>
    public class CoreJsonObject
    {
        private readonly JToken json;

        protected CoreJsonObject(JObject json)
        {
            this.json = json;
        }

        /// <summary>
        /// Getter for the stored JSON.
        /// </summary>
        public JToken GetStoredObject() => json;

        /// <summary>
        /// Return current JSON string.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(json, Formatting.None);
        }
    }
}
