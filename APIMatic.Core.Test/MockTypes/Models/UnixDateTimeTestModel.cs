using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Test.MockTypes.Convertors;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal class UnixDateTimeTestModel
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }
    }
}
