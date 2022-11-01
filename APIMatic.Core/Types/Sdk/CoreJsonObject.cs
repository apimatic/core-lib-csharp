// <copyright file="CoreJsonObject.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Types.Sdk
{
    public class CoreJsonObject
    {
        private readonly JToken json;

        protected CoreJsonObject(JObject json)
        {
            this.json = json;
        }

        /// <summary>
        /// Return current JSON string.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(json, Formatting.None);
        }

        /// <summary>
        /// Getter for the stored JSON.
        /// </summary>
        public JToken GetStoredObject()
        {
            return json;
        }
    }
}
