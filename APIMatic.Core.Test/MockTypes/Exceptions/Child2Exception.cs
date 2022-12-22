// <copyright file="Child2Exception.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Http;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Exceptions
{
    public class Child2Exception : ApiException
    {
        public Child2Exception(string reason, HttpContext context = null) : base(reason, context) { }

        /// <summary>
        /// Gets or sets Body.
        /// </summary>
        [JsonProperty("body c2")]
        public Dictionary<string, object> BodyC2 { get; set; }
    }
}
