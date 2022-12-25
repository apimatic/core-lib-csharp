using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Test.MockTypes.Convertors;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Date
{
    [TestFixture]
    internal class CoreListDateTimeConverterTest
    {
        [Test]
        public void SerializeListOfDateTime()
        {
            TestModelListOfDateTime testModelListOfDateTime = new TestModelListOfDateTime()
            {
                DateTimes = new List<DateTime> { new DateTime(2017, 1, 18, 6, 3, 1) }
            };

            string actual = CoreHelper.JsonSerialize(testModelListOfDateTime);
            string expected = "{\"DateTimes\":[\"2017-01-18T06:03:01\"],\"DateTimeOffsets\":null}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerializeListOfDateTimeOffset()
        {
            TestModelListOfDateTime testModelListOfDateTime = new TestModelListOfDateTime()
            {
               DateTimeOffsets= new List<DateTimeOffset> { new DateTimeOffset(new DateTime(2017, 1, 18, 6,3, 1))}
            };

            string actual = CoreHelper.JsonSerialize(testModelListOfDateTime);
            Assert.IsNotEmpty(actual);
        }

        [Test]
        public void DeSerializeListOfDateTime()
        {
            TestModelListOfDateTime expected = new TestModelListOfDateTime()
            {
                DateTimes = new List<DateTime> { new DateTime(2017, 1, 18, 6, 3, 1) }
            };
            string listOfDateTime = "{\"DateTimes\":[\"2017-01-18T06:03:01\"],\"DateTimeOffsets\":null}";
            TestModelListOfDateTime actual = CoreHelper.JsonDeserialize<TestModelListOfDateTime>(listOfDateTime, new ListDateTimeConverter());
            Assert.AreEqual(expected.DateTimes, actual.DateTimes);
        }

        [Test]
        public void DeSerializeListOfDateTimeOffset()
        {
            TestModelListOfDateTime expected = new TestModelListOfDateTime()
            {
                DateTimeOffsets = new List<DateTimeOffset> { new DateTimeOffset(new DateTime(2017, 1, 18, 6, 3, 1)) }
            };
            string listOfDateTime = "{\"DateTimes\":null,\"DateTimeOffsets\":[\"2017-01-18T06:03:01\"]}";
            TestModelListOfDateTime actual = CoreHelper.JsonDeserialize<TestModelListOfDateTime>(listOfDateTime, new ListDateTimeConverter());
            Assert.IsNotNull(actual.DateTimeOffsets);
            Assert.IsNull(actual.DateTimes);
        }
    }
}
