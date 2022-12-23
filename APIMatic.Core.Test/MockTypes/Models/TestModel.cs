using System;
using System.IO;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal class TestModel
    {
        public Stream DateStream { get; set; }

        public DateTime TestDateTime { get; set; }

        public int[] Integers { get; set; }
    }
}
