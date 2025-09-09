using System;
using APIMatic.Core.Test.MockTypes.Convertors;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal sealed class TestModelB
    {
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy'-'MM'-'dd")]
        public DateTime TestDateTime { get; set; }

    }
}
