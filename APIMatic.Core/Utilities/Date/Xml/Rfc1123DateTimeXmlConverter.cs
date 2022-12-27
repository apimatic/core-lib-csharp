using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace APIMatic.Core.Utilities.Date.Xml
{
    /// <summary>
    /// CoreRfc1123DateTimeXmlUtility contains a bunch of utility methods.
    /// </summary>
    public class Rfc1123DateTimeXmlConverter
    {
        /// <summary>
        /// Converts given date string to DateTime as per RFC 1123 time format.
        /// </summary>
        /// <param name="date">Date time as string.</param>
        /// <returns>Datetime object.</returns>
        public static DateTime? StringToRfc1123Date(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            DateTime rfc1123DateTime;
            var provider = CultureInfo.InvariantCulture;

            rfc1123DateTime = DateTime.ParseExact(date, "r", provider).ToLocalTime();
            return rfc1123DateTime;
        }

        /// <summary>
        /// Converts given DateTime to string as per RFC 1123 time format.
        /// </summary>
        /// <param name="date">DateTime object.</param>
        /// <returns>Date time as string.</returns>
        public static string Rfc1123DateToString(DateTime? date)
        {
            return date?.ToUniversalTime().ToString("r");
        }

        /// <summary>
        /// Converts given XML string to DateTime as per RFC 1123 time format.
        /// </summary>
        /// <param name="date">Date time as string.</param>
        /// <returns>Datetime object.</returns>
        public static DateTime? FromRfc1123DateTimeXml(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            return StringToRfc1123Date(XDocument.Parse(date).Root.Value);
        }

        /// <summary>
        /// Extracts DateTime list from the given XML string as per RFC 1123 time
        /// format.
        /// </summary>
        /// <param name="dates">Dates as string.</param>
        /// <returns>List of DateTime objects.</returns>
        public static List<DateTime> FromRfc1123DateTimeListXml(string dates)
        {
            if (string.IsNullOrWhiteSpace(dates))
            {
                return null;
            }

            return XDocument.Parse(dates).Root.Elements()
                .Select(e => StringToRfc1123Date(e.Value).GetValueOrDefault())
                .ToList();
        }

        /// <summary>
        /// Converts given DateTime to XML string as per RFC 1123 time format.
        /// </summary>
        /// <param name="date">DateTime object.</param>
        /// <param name="rootName">Root name.</param>
        /// <returns>Date time as string.</returns>
        public static string ToRfc1123DateTimeXml(DateTime? date, string rootName = null)
        {
            return XmlPrinter.Print(date, Rfc1123DateToString, rootName ?? "DateTime");
        }

        /// <summary>
        /// Converts given DateTime data to XML string as per RFC 1123 time format.
        /// </summary>
        /// <param name="dates">Dates enumeration.</param>
        /// <param name="rootName">Root name.</param>
        /// <param name="arrayNodeName">Node name.</param>
        /// <param name="arrayItemName">Item name.</param>
        /// <returns>Date time as string.</returns>
        public static string ToRfc1123DateTimeListXml(IEnumerable<DateTime?> dates, string rootName = null, string arrayNodeName = null, string arrayItemName = null)
        {
            return XmlPrinter.PrintArray(dates, Rfc1123DateToString, rootName ?? "DateTime", arrayItemName ?? "dateTime", arrayNodeName);
        }
    }
}
