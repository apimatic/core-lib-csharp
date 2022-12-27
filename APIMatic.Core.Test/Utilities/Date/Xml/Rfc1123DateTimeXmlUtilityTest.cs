using System;
using System.Collections.Generic;
using System.Globalization;
using APIMatic.Core.Utilities.Date.Xml;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Date.Xml
{
    [TestFixture]
    internal class Rfc1123DateTimeXmlUtilityTest
    {
        [Test]
        public void StringToRfc1123Date_WithNullString()
        {
            DateTime? dateTime = Rfc1123DateTimeXmlConverter.StringToRfc1123Date(null);
            Assert.IsNull(dateTime);
        }

        [Test]
        public void StringToRfc1123Date_WithValidString()
        {
            string datetimeString = "Wed, 18 Jan 2017 01:01:03 GMT";
            var expected = StringToRfc1123DateTime(datetimeString);
            DateTime? actual = Rfc1123DateTimeXmlConverter.StringToRfc1123Date(datetimeString);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Rfc1123DateToString_WithValidDate()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 1, 3);
            var expected = Rfc1123DateTimeToString(dateTime);
            string actual = Rfc1123DateTimeXmlConverter.Rfc1123DateToString(dateTime);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToRfc1123DateTimeXml_WithValidDate()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 1, 3);
            string expectedDateTime = Rfc1123DateTimeToString(dateTime);
            string expected = $"<DateTime>{expectedDateTime}</DateTime>";
            string actual = Rfc1123DateTimeXmlConverter.ToRfc1123DateTimeXml(dateTime);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromRfc1123DateTimeXml_WithValidString()
        {
            string dateTime = "Wed, 18 Jan 2017 01:01:03 GMT";
            string dateTimeString = $"<DateTime>{dateTime}</DateTime>";
            DateTime? expected = StringToRfc1123DateTime(dateTime);
            DateTime? actual = Rfc1123DateTimeXmlConverter.FromRfc1123DateTimeXml(dateTimeString);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromRfc1123DateTimeXml_WithInValidString()
        {
            string dateTimeString = "<DateTime></DateTime>";
            DateTime? actual = Rfc1123DateTimeXmlConverter.FromRfc1123DateTimeXml(dateTimeString);
            Assert.IsNull(actual);
        }

        [Test]
        public void FromRfc1123DateTimeXml_WithEmptydString()
        {
            string dateTimeString = string.Empty;
            DateTime? actual = Rfc1123DateTimeXmlConverter.FromRfc1123DateTimeXml(dateTimeString);
            Assert.IsNull(actual);
        }

        [Test]
        public void ToRfc1123DateTimeListXml_WithInValidDate()
        {
            List<DateTime> dateTimes = null;
            string expected = "<DateTime />";
            string actual = Rfc1123DateTimeXmlConverter.ToRfc1123DateTimeListXml(dateTimes);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToRfc1123DateTimeXml_WithValidDateArrayNodeName()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 3, 1);
            List<DateTime> dateTimes = new List<DateTime>
            {
                dateTime,
                dateTime
            };
            string expectedDateTime = Rfc1123DateTimeToString(dateTime);
            string expected = $"<dateTime>\n  <dateTime>{expectedDateTime}</dateTime>\n  <dateTime>{expectedDateTime}</dateTime>\n</dateTime>";
            string actual = StringReplacer.ReplaceBackSlashR(Rfc1123DateTimeXmlConverter.ToRfc1123DateTimeListXml(dateTimes, arrayNodeName: "dateTime"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToRfc1123DateTimeXml_WithValidDateWithoutArrayNode()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 3, 1);
            List<DateTime> dateTimes = new List<DateTime>
            {
                dateTime,
                dateTime
            };
            string expectedDateTime = Rfc1123DateTimeToString(dateTime);
            string expected = $"<DateTime>\n  <dateTime>{expectedDateTime}</dateTime>\n  <dateTime>{expectedDateTime}</dateTime>\n</DateTime>";
            string actual = StringReplacer.ReplaceBackSlashR(Rfc1123DateTimeXmlConverter.ToRfc1123DateTimeListXml(dateTimes));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromRfc1123DateTimeListXml_WithValidString()
        {
            string rfc1123DateTimeString = "Wed, 18 Jan 2017 01:03:01 GMT";
            DateTime dateTime = StringToRfc1123DateTime(rfc1123DateTimeString);
            List<DateTime> expected = new List<DateTime>
            {
                dateTime,
                dateTime
            };
            string rfc1123DateTime = "<DateTime>\r\n  <dateTime>Wed, 18 Jan 2017 01:03:01 GMT</dateTime>\r\n  <dateTime>Wed, 18 Jan 2017 01:03:01 GMT</dateTime>\r\n</DateTime>";
            List<DateTime> actual = Rfc1123DateTimeXmlConverter.FromRfc1123DateTimeListXml(rfc1123DateTime);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromRfc1123DateTimeListXml_WithEmptyString()
        {
            string rfc1123DateTime = string.Empty;
            List<DateTime> actual = Rfc1123DateTimeXmlConverter.FromRfc1123DateTimeListXml(rfc1123DateTime);
            Assert.IsNull(actual);
        }

        private static DateTime StringToRfc1123DateTime(string datetimeString)
        {
            DateTime expected;
            var provider = CultureInfo.InvariantCulture;

            expected = DateTime.ParseExact(datetimeString, "r", provider).ToLocalTime();
            return expected;
        }

        private static string Rfc1123DateTimeToString(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("r");
        }
    }
}
