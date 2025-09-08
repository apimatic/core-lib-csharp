using System;
using System.Collections.Generic;
using System.Globalization;
using APIMatic.Core.Test.MockTypes.Convertors;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Date
{
    [TestFixture]
    internal sealed class CoreUnixDateTimeConvertorTest
    {
        [Test]
        public void UnixDateTimeConverter_ReadJson()
        {
            var expected = new List<DateTime>
            {
                new DateTime(2017, 1, 18, 6, 3, 1, DateTimeKind.Utc),
                new DateTime(2017, 1, 18, 6, 3, 1, DateTimeKind.Utc)
            };
            var unixDateTimeConverter = new UnixDateTimeConverter
            {
                DateTimeStyles = DateTimeStyles.AssumeUniversal
            };
            var actual = CoreHelper.JsonDeserialize<List<DateTime>>(
                    "[1484719381,1484719381]", unixDateTimeConverter);
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(unixDateTimeConverter.CanRead);
            Assert.IsTrue(unixDateTimeConverter.DateTimeStyles.HasFlag(DateTimeStyles.AssumeUniversal));
        }

        [Test]
        public void UnixDateTimeConverter_ReadJsonNull()
        {
            var unixDateTimeConverter = new UnixDateTimeConverter();
            var exception = Assert.Throws<ArgumentNullException>(() => CoreHelper.JsonDeserialize<List<DateTime>>(
                    "[null]", unixDateTimeConverter));
            var expectedMessage = "Value cannot be null. (Parameter 'item')";
            var actualMessage = exception.Message;
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [Test]
        public void UnixDateTimeConverter_JsonReaderException()
        {
            var unixDateTimeConverter = new UnixDateTimeConverter();
            var exception = Assert.Throws<JsonSerializationException>(() => CoreHelper.JsonDeserialize<List<DateTime>>(
                    "[\"testCase\",1484719381]", unixDateTimeConverter));
            var expectedMessage = "Unexpected token";
            var actualMessage = exception.Message;
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [Test]
        public void UnixDateTimeConverter_AssumeUniversalDateTimeStyles()
        {
            var unixDateTimeConverter = new UnixDateTimeConverter
            {
                DateTimeStyles = DateTimeStyles.AssumeUniversal
            };
            var listOfDateTime = CoreHelper.JsonDeserialize<List<DateTime>>(
                    "[1484719381,1484719381]", unixDateTimeConverter);
            var expected = "[1484719381.0,1484719381.0]";
            var actual = CoreHelper.JsonSerialize(listOfDateTime, unixDateTimeConverter);
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(unixDateTimeConverter.CanRead);
            Assert.IsTrue(unixDateTimeConverter.DateTimeStyles.HasFlag(DateTimeStyles.AssumeUniversal));
        }

        [Test]
        public void UnixDateTimeConverter_AdjustToUniversalDateTimeStyles()
        {
            var unixDateTimeConverter = new UnixDateTimeConverter
            {
                DateTimeStyles = DateTimeStyles.AdjustToUniversal
            };
            var listOfDateTime = CoreHelper.JsonDeserialize<List<DateTime>>(
                    "[1484719381,1484719381]", unixDateTimeConverter);
            var expected = "[1484719381.0,1484719381.0]";
            var actual = CoreHelper.JsonSerialize(listOfDateTime, unixDateTimeConverter);
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(unixDateTimeConverter.CanRead);
            Assert.IsTrue(unixDateTimeConverter.DateTimeStyles.HasFlag(DateTimeStyles.AdjustToUniversal));
        }

        [Test]
        public void UnixDateTimeConverter_ModelProperty()
        {
            var unixDateTimeTestModel = new UnixDateTimeTestModel()
            {
                DateTime = new DateTime(2017, 1, 18, 6, 3, 1, DateTimeKind.Utc)
            };
            // Deserialize expected output
            var expected = "{\"dateTime\":1484719381.0}";
            var actual = CoreHelper.JsonSerialize(
                    unixDateTimeTestModel);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UnixDateTimeConverter_InvalidDate()
        {
            var unixDateTimeTestModelFake = new UnixDateTimeTestModelFake()
            {
                DateTime = 12
            };
            var exception = Assert.Throws<JsonSerializationException>(() => CoreHelper.JsonSerialize(
                    unixDateTimeTestModelFake));
            var expectedMessage = "Unexpected value when converting date. Expected DateTime.";
            var actualMessage = exception.Message;
            Assert.AreEqual(expectedMessage, actualMessage);
        }
    }
}
