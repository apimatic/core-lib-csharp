using APIMatic.Core.Test.MockTypes.Convertors;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal sealed class UnixDateTimeTestModelFake
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("dateTime")]
        public int DateTime { get; set; }
    }
}
