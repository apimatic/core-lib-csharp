// <copyright file="Child1Exception.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Http;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Exceptions
{
    public class Child1Exception : ApiException
    {
        public Child1Exception(string reason, HttpContext context = null) : base(reason, context) { }

        /// <summary>
        /// Gets or sets Body.
        /// </summary>
        [JsonProperty("body c1")]
        public Dictionary<string, object> BodyC1 { get; set; }
    }
}
