using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal sealed class TestModelListOfDateTime
    {
        [JsonConverter(typeof(CoreListConverter), typeof(IsoDateTimeConverter))]
        public List<DateTime> DateTimes { get; set; }

        [JsonConverter(typeof(CoreListConverter), typeof(IsoDateTimeConverter))]
        public List<DateTimeOffset> DateTimeOffsets { get; set; }
    }
}
