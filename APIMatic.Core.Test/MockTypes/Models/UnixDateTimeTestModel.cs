using System;
using APIMatic.Core.Test.MockTypes.Convertors;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal sealed class UnixDateTimeTestModel
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }
    }
}
