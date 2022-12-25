using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Test.MockTypes.Convertors;
using APIMatic.Core.Utilities.Date;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal class TestModelForMapOfDateTime
    {
        [JsonConverter(typeof(MapDateTimeConverter))]
        public Dictionary<string, DateTime> DateTimePairs { get; set; }
    }
}
