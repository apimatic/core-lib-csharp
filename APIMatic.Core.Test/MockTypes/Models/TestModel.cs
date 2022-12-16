using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Utilities.Date;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal class TestModel
    {
        public Stream DateStream { get; set; } = null;

        public DateTime TestDateTime { get; set; }

        public int[] Integers { get; set; }
    }
}
