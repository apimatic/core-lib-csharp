using System;
using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Convertors;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal class TestModelForMapOfDateTime
    {
        [JsonConverter(typeof(MapDateTimeConverter))]
        public Dictionary<string, DateTime> DateTimePairs { get; set; }
    }
}
