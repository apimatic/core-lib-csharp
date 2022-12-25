using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            DateTime expected = new DateTime(2017, 1, 18, 6, 1, 3);
            string datetimeString = "Wed, 18 Jan 2017 01:01:03 GMT";
            DateTime? actual = Rfc1123DateTimeXmlConverter.StringToRfc1123Date(datetimeString);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Rfc1123DateToString_WithValidDate()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 1, 3);
            string expected = "Wed, 18 Jan 2017 01:01:03 GMT";
            string actual = Rfc1123DateTimeXmlConverter.Rfc1123DateToString(dateTime);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToRfc1123DateTimeXml_WithValidDate()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 1, 3);
            string expected = "<DateTime>Wed, 18 Jan 2017 01:01:03 GMT</DateTime>";
            string actual = Rfc1123DateTimeXmlConverter.ToRfc1123DateTimeXml(dateTime);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromRfc1123DateTimeXml_WithValidString()
        {
            string dateTimeString = "<DateTime>Wed, 18 Jan 2017 01:01:03 GMT</DateTime>";
            DateTime? expected = new DateTime(2017, 1, 18, 6, 1, 3);
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
            List<DateTime> dateTimes = new List<DateTime>
            {
                new DateTime(2017, 1, 18, 6, 3, 1),
                new DateTime(2017, 1, 18, 6, 3, 1)
            };
            string expected = "<DateTime>\r\n  <dateTime>\r\n    <dateTime>Wed, 18 Jan 2017 01:03:01 GMT</dateTime>\r\n    <dateTime>Wed, 18 Jan 2017 01:03:01 GMT</dateTime>\r\n  </dateTime>\r\n</DateTime>";
            expected = StringReplacer.ReplaceBackSlashR(expected);
            string actual = Rfc1123DateTimeXmlConverter.ToRfc1123DateTimeListXml(dateTimes, arrayNodeName: "dateTime");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToRfc1123DateTimeXml_WithValidDateWithoutArrayNode()
        {
            List<DateTime> dateTimes = new List<DateTime>
            {
                new DateTime(2017, 1, 18, 6, 3, 1),
                new DateTime(2017, 1, 18, 6, 3, 1)
            };
            string expected = "<DateTime>\r\n  <dateTime>Wed, 18 Jan 2017 01:03:01 GMT</dateTime>\r\n  <dateTime>Wed, 18 Jan 2017 01:03:01 GMT</dateTime>\r\n</DateTime>";
            expected = StringReplacer.ReplaceBackSlashR(expected);
            string actual = Rfc1123DateTimeXmlConverter.ToRfc1123DateTimeListXml(dateTimes);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromRfc1123DateTimeListXml_WithValidString()
        {
            List<DateTime> expected = new List<DateTime>
            {
                new DateTime(2017, 1, 18, 6, 3, 1),
                new DateTime(2017, 1, 18, 6, 3, 1)
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
    }
}
