﻿using System;
using System.Collections.Generic;
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
