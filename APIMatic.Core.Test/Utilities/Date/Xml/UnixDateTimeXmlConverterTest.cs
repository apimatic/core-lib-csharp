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
            var expected = UnixDateToString(dateTime);
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
            string unixDatetimeString = "1484701381";
            var expected = StringToUnixDateTime(unixDatetimeString);
            DateTime? actual = UnixDateTimeXmlConverter.StringToUnixDate(unixDatetimeString);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromUnixDateTimeXml_WithDateXMLString()
        {
            string unixDateTimeString = "1484701381";
            string dateString = $"<dateTime>{unixDateTimeString}</dateTime>";
            DateTime expected = StringToUnixDateTime(unixDateTimeString);
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
            string expectedDateTimeString = UnixDateToString(dateTime);
            string expected = $"<DateTime>{expectedDateTimeString}</DateTime>";
            string actual = UnixDateTimeXmlConverter.ToUnixDateTimeXml(dateTime);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToUnixDateTimeListXml_WithXMLDates()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 3, 1);
            var dateTimes = new List<DateTime>
            {
                dateTime,
                dateTime
            };
            string expectedDateTimeString = UnixDateToString(dateTime);
            string expected = $"<dateTimes>\n  <dateTime>{expectedDateTimeString}</dateTime>\n  <dateTime>{expectedDateTimeString}</dateTime>\n</dateTimes>";
            string actual = StringReplacer.ReplaceBackSlashR(UnixDateTimeXmlConverter.ToUnixDateTimeListXml(dateTimes, "dateTimes"));
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
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 3, 1);
            var dateTimes = new List<DateTime>
            {
                dateTime,
                dateTime
            };
            string expectedDateTimeString = UnixDateToString(dateTime);
            string expected = $"<DateTime>\n  <dateTime>\n    <dateTime>{expectedDateTimeString}</dateTime>\n    <dateTime>{expectedDateTimeString}</dateTime>\n  </dateTime>\n</DateTime>";
            string actual = StringReplacer.ReplaceBackSlashR(UnixDateTimeXmlConverter.ToUnixDateTimeListXml(dateTimes, null, "dateTime"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToUnixDateTimeListXml_WithRootName()
        {
            DateTime dateTime = new DateTime(2017, 1, 18, 6, 3, 1);
            var dateTimes = new List<DateTime>
            {
                dateTime,
                dateTime
            };
            string expectedDateTimeString = UnixDateToString(dateTime);
            string expected = $"<Unix>\n  <dateTime>\n    <dateTime>{expectedDateTimeString}</dateTime>\n    <dateTime>{expectedDateTimeString}</dateTime>\n  </dateTime>\n</Unix>";
            string actual = StringReplacer.ReplaceBackSlashR(UnixDateTimeXmlConverter.ToUnixDateTimeListXml(dateTimes, "Unix", "dateTime"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromUnixDateTimeListXml_WithDateXMLString()
        {
            string unixDateTime = "1484701381";
            string unixDateTimes = $"<dateTimes>\r\n  <dateTime>{unixDateTime}</dateTime>\r\n  <dateTime>{unixDateTime}</dateTime>\r\n</dateTimes>";
            var expectedDateTime = StringToUnixDateTime(unixDateTime);
            var expected = new List<DateTime>
            {
               expectedDateTime,
               expectedDateTime
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

        private static string UnixDateToString(DateTime dateTime)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime.ToUniversalTime());
            var expected = dateTimeOffset.ToUnixTimeSeconds().ToString();
            return expected;
        }

        private static DateTime StringToUnixDateTime(string unixDatetimeString)
        {
            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(unixDatetimeString)).DateTime.ToLocalTime();
        }
    }
}
