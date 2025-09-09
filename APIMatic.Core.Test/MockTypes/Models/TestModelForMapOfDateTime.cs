using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal sealed class TestModelForMapOfDateTime
    {
        [JsonConverter(typeof(CoreMapConverter), typeof(IsoDateTimeConverter))]
        public Dictionary<string, DateTime> DateTimePairs { get; set; }
    }
}
