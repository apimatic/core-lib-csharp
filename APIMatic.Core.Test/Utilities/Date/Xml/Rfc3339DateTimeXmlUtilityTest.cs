using System;
using System.Collections.Generic;
using System.Xml;
using APIMatic.Core.Utilities.Date.Xml;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Date.Xml
{
    [TestFixture]
    internal sealed class Rfc3339DateTimeXmlUtilityTest
    {
        [Test]
        public void StringToRfc3339Date_WithEmptyString()
        {
            DateTime? actual = Rfc3339DateTimeXmlConverter.StringToRfc3339Date(string.Empty);
            Assert.IsNull(actual);
        }

        [Test]
        public void StringToRfc3339Date_WithValidString()
        {
            string dateTimeString = "2017-01-18T06:01:03Z";
            var expected = StringToRfc3339DateTime(dateTimeString);
            DateTime? actual = Rfc3339DateTimeXmlConverter.StringToRfc3339Date(dateTimeString);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Rfc3339DateToString_WithValidDate()
        {
            DateTime? dateTime = new DateTime(2017, 1, 18, 6, 1, 3);
            var expected = Rfc3339ToString(dateTime);
            string actual = Rfc3339DateTimeXmlConverter.Rfc3339DateToString(dateTime);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Rfc3339DateToString_WithInValidDate()
        {
            DateTime? dateTime = null;
            string actual = Rfc3339DateTimeXmlConverter.Rfc3339DateToString(dateTime);
            Assert.IsNull(actual);
        }

        [Test]
        public void ToRfc3339DateTimeXml_WithValidDate()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 1, 3);
            var expectedDateTime = Rfc3339ToString(dateTime);
            string expected = $"<DateTime>{expectedDateTime}</DateTime>";
            string actual = Rfc3339DateTimeXmlConverter.ToRfc3339DateTimeXml(dateTime);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void ToRfc3339DateTimeXml_WithInValidDate()
        {
            DateTime? dateTime = null;
            string actual = Rfc3339DateTimeXmlConverter.ToRfc3339DateTimeXml(dateTime);
            Assert.IsNull(actual);
        }

        [Test]
        public void FromRfc3339DateTimeXmll_WithValidString()
        {
            string rfc3339DateTime = "2017-01-18T06:01:03Z";
            string rfc3399Xml = $"<DateTime>{rfc3339DateTime}</DateTime>";
            DateTime expected = StringToRfc3339DateTime(rfc3339DateTime);
            DateTime? actual = Rfc3339DateTimeXmlConverter.FromRfc3339DateTimeXml(rfc3399Xml);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromRfc3339DateTimeXmll_WithEmptyBlockString()
        {
            string rfc3399Xml = "<DateTime></DateTime>";
            DateTime? actual = Rfc3339DateTimeXmlConverter.FromRfc3339DateTimeXml(rfc3399Xml);
            Assert.IsNull(actual);
        }

        [Test]
        public void FromRfc3339DateTimeXmll_WithNullString()
        {
            string rfc3399Xml = string.Empty;
            DateTime? actual = Rfc3339DateTimeXmlConverter.FromRfc3339DateTimeXml(rfc3399Xml);
            Assert.IsNull(actual);
        }

        [Test]
        public void ToRfc3339DateTimeXml_WithDate()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 3, 1);
            List<DateTime?> dateTimes = new List<DateTime?>
            {
                dateTime,
                dateTime
            };
            string rfcDateTimeString = Rfc3339ToString(dateTime);
            string expected = $"<DateTime>\n  <dateTime>{rfcDateTimeString}</dateTime>\n  <dateTime>{rfcDateTimeString}</dateTime>\n</DateTime>";
            string actual = StringReplacer.ReplaceBackSlashR(Rfc3339DateTimeXmlConverter.ToRfc3339DateTimeListXml(dateTimes));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToRfc3339DateTimeXml_WithDateArrayNodeName()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 3, 1);
            List<DateTime?> dateTimes = new List<DateTime?>
            {
                dateTime,
                dateTime
            };
            string rfcDateTimeString = Rfc3339ToString(dateTime);
            string expected = $"<DateTime>\n  <dateTime>\n    <dateTime>{rfcDateTimeString}</dateTime>\n    <dateTime>{rfcDateTimeString}</dateTime>\n  </dateTime>\n</DateTime>";
            string actual = StringReplacer.ReplaceBackSlashR(Rfc3339DateTimeXmlConverter.ToRfc3339DateTimeListXml(dateTimes, arrayNodeName: "dateTime"));
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void ToRfc3339DateTimeXml_WithNullDates()
        {
            List<DateTime?> dateTimes = null;

            string expected = "<DateTime />";
            string actual = Rfc3339DateTimeXmlConverter.ToRfc3339DateTimeListXml(dateTimes);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromRfc3339DateTimeListXml_WithValidStringDate()
        {
            string rfc3339DateTime = "2017-01-18T06:03:01Z";
            DateTime dateTime = StringToRfc3339DateTime(rfc3339DateTime);
            string rfc3339DateTimes = "<DateTime>\r\n  <dateTime>2017-01-18T06:03:01Z</dateTime>\r\n  <dateTime>2017-01-18T06:03:01Z</dateTime>\r\n</DateTime>";
            List<DateTime> expected = new List<DateTime>
            {
                dateTime,
                dateTime
            };
            List<DateTime> actual = Rfc3339DateTimeXmlConverter.FromRfc3339DateTimeListXml(rfc3339DateTimes);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromRfc3339DateTimeListXml_WithInValidStringDate()
        {
            string rfc3339DateTimes = string.Empty;
            List<DateTime> actual = Rfc3339DateTimeXmlConverter.FromRfc3339DateTimeListXml(rfc3339DateTimes);
            Assert.IsNull(actual);
        }

        private static DateTime StringToRfc3339DateTime(string dateTimeString)
        {
            return XmlConvert.ToDateTime(dateTimeString, XmlDateTimeSerializationMode.Utc);
        }

        private static string Rfc3339ToString(DateTime? dateTime)
        {
            return XmlConvert.ToString(dateTime.GetValueOrDefault(), XmlDateTimeSerializationMode.Utc);
        }
    }
}
