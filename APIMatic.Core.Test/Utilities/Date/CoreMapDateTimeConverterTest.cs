using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Test.MockTypes.Convertors;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;
using APIMatic.Core.Utilities.Date;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Date
{
    [TestFixture]
    internal class CoreMapDateTimeConverterTest
    {
        [Test]
        public void SerializeMapOfDateTime()
        {
            TestModelForMapOfDateTime testModelForMapOfDateTime = new TestModelForMapOfDateTime()
            {
                DateTimePairs = new Dictionary<string, DateTime> { { "date1", new DateTime(2017, 1, 18, 6, 3, 1) } }
            };

            string actual = CoreHelper.JsonSerialize(testModelForMapOfDateTime);
            string expected = "{\"DateTimePairs\":{\"date1\":\"2017-01-18T06:03:01\"}}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeserializeMapOfDateTime()
        {
            TestModelForMapOfDateTime expected = new TestModelForMapOfDateTime()
            {
                DateTimePairs = new Dictionary<string, DateTime> { { "date1", new DateTime(2017, 1, 18, 6, 3, 1) } }
            };

            string mapOfDateTimeString = "{\"DateTimePairs\":{\"date1\":\"2017-01-18T06:03:01\"}}";

            TestModelForMapOfDateTime actual = CoreHelper.JsonDeserialize<TestModelForMapOfDateTime>(mapOfDateTimeString, new MapDateTimeConverter());
            Assert.AreEqual(expected.DateTimePairs, actual.DateTimePairs);
        }
    }
}
