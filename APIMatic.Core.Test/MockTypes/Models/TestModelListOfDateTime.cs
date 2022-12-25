using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Test.MockTypes.Convertors;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal class TestModelListOfDateTime
    {
        [JsonConverter(typeof(ListDateTimeConverter))]
        public List<DateTime> DateTimes { get; set; }  
        
        [JsonConverter(typeof(ListDateTimeConverter))]
        public List<DateTimeOffset> DateTimeOffsets { get; set; }

        
    }
}
