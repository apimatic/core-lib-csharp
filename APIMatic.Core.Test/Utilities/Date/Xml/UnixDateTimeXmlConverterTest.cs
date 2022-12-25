using System;
using System.Collections.Generic;
using APIMatic.Core.Utilities.Date.Xml;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Date.Xml
{
    [TestFixture]
    internal class UnixDateTimeXmlConverterTest
    {
        [Test]
        public void UnixDateToString_WithDate()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 3, 1);
            string expected = "1484701381";
            string actual = UnixDateTimeXmlConverter.UnixDateToString(dateTime);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StringToUnixDate_WithNullString()
        {
            DateTime? actual = UnixDateTimeXmlConverter.StringToUnixDate("");
            Assert.IsNull(actual);
        }

        [Test]
        public void StringToUnixDate_WithDateString()
        {
            string unixDatetime = "1484701381";
            DateTime expected = new DateTime(2017, 1, 18, 6, 3, 1);
            DateTime? actual = UnixDateTimeXmlConverter.StringToUnixDate(unixDatetime);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void FromUnixDateTimeXml_WithDateXMLString()
        {
            string dateString = "<dateTime>1484701381</dateTime>";
            DateTime expected = new DateTime(2017, 1, 18, 6, 3, 1);
            DateTime? actual = UnixDateTimeXmlConverter.FromUnixDateTimeXml(dateString);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromUnixDateTimeXml_WithDateNullDateXML()
        {
            string dateString = "<dateTime></dateTime>";
            DateTime? actual = UnixDateTimeXmlConverter.FromUnixDateTimeXml(dateString);
            Assert.IsNull(actual);
        }

        [Test]
        public void FromUnixDateTimeXml_WithNullString()
        {
            string dateString = string.Empty;
            DateTime? actual = UnixDateTimeXmlConverter.FromUnixDateTimeXml(dateString);
            Assert.IsNull(actual);
        }

        [Test]
        public void ToUnixDateTimeXml_WithDateXMLString()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 3, 1);
            string expected = "<DateTime>1484701381</DateTime>";
            string actual = UnixDateTimeXmlConverter.ToUnixDateTimeXml(dateTime);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToUnixDateTimeListXml_WithXMLDates()
        {
            var dateTimes = new List<DateTime>
            {
                new DateTime(2017, 1, 18, 6, 3, 1),
                new DateTime(2017, 1, 18, 6, 3, 1)
            };
            string expected = "<dateTimes>\r\n  <dateTime>1484701381</dateTime>\r\n  <dateTime>1484701381</dateTime>\r\n</dateTimes>";
            expected = StringReplacer.ReplaceBackSlashR(expected);
            string actual = UnixDateTimeXmlConverter.ToUnixDateTimeListXml(dateTimes, "dateTimes");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToUnixDateTimeListXml_WithXMLNullDates()
        {
            List<DateTime> dateTimes = null;
            string expected = "<dateTimes />";
            string actual = UnixDateTimeXmlConverter.ToUnixDateTimeListXml(dateTimes, "dateTimes");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToUnixDateTimeListXml_XMLDatesWithArrayNodes()
        {
            var dateTimes = new List<DateTime>
            {
                new DateTime(2017, 1, 18, 6, 3, 1),
                new DateTime(2017, 1, 18, 6, 3, 1)
            };
            string expected = "<DateTime>\r\n  <dateTime>\r\n    <dateTime>1484701381</dateTime>\r\n    <dateTime>1484701381</dateTime>\r\n  </dateTime>\r\n</DateTime>";
            expected = StringReplacer.ReplaceBackSlashR(expected);
            string actual = UnixDateTimeXmlConverter.ToUnixDateTimeListXml(dateTimes, null, "dateTime");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToUnixDateTimeListXml_WithRootName()
        {
            var dateTimes = new List<DateTime>
            {
                new DateTime(2017, 1, 18, 6, 3, 1),
                new DateTime(2017, 1, 18, 6, 3, 1)
            };
            string expected = "<Unix>\r\n  <dateTime>\r\n    <dateTime>1484701381</dateTime>\r\n    <dateTime>1484701381</dateTime>\r\n  </dateTime>\r\n</Unix>";
            expected = StringReplacer.ReplaceBackSlashR(expected);
            string actual = UnixDateTimeXmlConverter.ToUnixDateTimeListXml(dateTimes, "Unix", "dateTime");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromUnixDateTimeListXml_WithDateXMLString()
        {
            string unixDateTimes = "<dateTimes>\r\n  <dateTime>1484701381</dateTime>\r\n  <dateTime>1484701381</dateTime>\r\n</dateTimes>";
            var expected = new List<DateTime>
            {
                new DateTime(2017, 1, 18, 6, 3, 1),
                new DateTime(2017, 1, 18, 6, 3, 1)
            };
            List<DateTime> actual = UnixDateTimeXmlConverter.FromUnixDateTimeListXml(unixDateTimes);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromUnixDateTimeListXml_NullString()
        {
            string unixDateTimes = string.Empty;
            List<DateTime> actual = UnixDateTimeXmlConverter.FromUnixDateTimeListXml(unixDateTimes);
            Assert.IsNull(actual);
        }
    }
}
