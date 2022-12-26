using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities.Date.Xml;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Date.Xml
{
    [TestFixture]
    internal class Rfc3339DateTimeXmlUtilityTest
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
            DateTime expected = new DateTime(2017, 1, 18, 6, 1, 3);
            string dateTimeString = "2017-01-18T06:01:03Z";
            DateTime? actual = Rfc3339DateTimeXmlConverter.StringToRfc3339Date(dateTimeString);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Rfc3339DateToString_WithValidDate()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 1, 3);
            string expected = "2017-01-18T06:01:03Z";
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
            string expected = "<DateTime>2017-01-18T06:01:03Z</DateTime>";
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
            DateTime expected = new DateTime(2017, 1, 18, 6, 1, 3);
            string rfc3399Xml = "<DateTime>2017-01-18T06:01:03Z</DateTime>";
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
            List<DateTime?> dateTimes = new List<DateTime?>
            {
                new DateTime(2017, 1, 18, 6, 3, 1),
                new DateTime(2017, 1, 18, 6, 3, 1)
            };

            string expected = "<DateTime>\n  <dateTime>2017-01-18T06:03:01Z</dateTime>\n  <dateTime>2017-01-18T06:03:01Z</dateTime>\n</DateTime>";
            string actual = StringReplacer.ReplaceBackSlashR(Rfc3339DateTimeXmlConverter.ToRfc3339DateTimeListXml(dateTimes));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToRfc3339DateTimeXml_WithDateArrayNodeName()
        {
            List<DateTime?> dateTimes = new List<DateTime?>
            {
                new DateTime(2017, 1, 18, 6, 3, 1),
                new DateTime(2017, 1, 18, 6, 3, 1)
            };

            string expected = "<DateTime>\n  <dateTime>\n    <dateTime>2017-01-18T06:03:01Z</dateTime>\n    <dateTime>2017-01-18T06:03:01Z</dateTime>\n  </dateTime>\n</DateTime>";
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
            string rfc3339DateTimes = "<DateTime>\r\n  <dateTime>2017-01-18T06:03:01Z</dateTime>\r\n  <dateTime>2017-01-18T06:03:01Z</dateTime>\r\n</DateTime>";
            List<DateTime> expected = new List<DateTime>
            {
                new DateTime(2017, 1, 18, 6, 3, 1, DateTimeKind.Utc),
                new DateTime(2017, 1, 18, 6, 3, 1, DateTimeKind.Utc)
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
    }
}
