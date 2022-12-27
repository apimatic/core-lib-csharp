// <copyright file="Rfc3339DateTimeXmlConverter.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace APIMatic.Core.Utilities.Date.Xml
{
    /// <summary>
    /// CoreRfc3339DateTimeXmlUtility contains a bunch of utility methods.
    /// </summary>
    public class Rfc3339DateTimeXmlConverter
    {
        /// <summary>
        /// Converts given date string to DateTime as per RFC 3339 time format.
        /// </summary>
        /// <param name="date">Date time as string.</param>
        /// <returns>Datetime object.</returns>
        public static DateTime? StringToRfc3339Date(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            var rfc3339DateTime = XmlConvert.ToDateTime(date, XmlDateTimeSerializationMode.Utc);
            return rfc3339DateTime;
        }

        /// <summary>
        /// Converts given DateTime to string as per RFC 3339 time format.
        /// </summary>
        /// <param name="date">DateTime object.</param>
        /// <returns>Date time as string.</returns>
        public static string Rfc3339DateToString(DateTime? date)
        {
            if (date == null)
            {
                return null;
            }

            var rfc3339DateTime = XmlConvert.ToString(date.GetValueOrDefault(), XmlDateTimeSerializationMode.Utc);
            return rfc3339DateTime;
        }

        /// <summary>
        /// Converts given XML string to DateTime as per RFC 3339 time format.
        /// </summary>
        /// <param name="date">Date time as string.</param>
        /// <returns>Datetime object.</returns>
        public static DateTime? FromRfc3339DateTimeXml(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            return StringToRfc3339Date(XDocument.Parse(date).Root.Value);
        }

        /// <summary>
        /// Extracts DateTime list from the given XML string as per RFC 3339 time
        /// format.
        /// </summary>
        /// <param name="dates">Dates as string.</param>
        /// <returns>List of DateTime objects.</returns>
        public static List<DateTime> FromRfc3339DateTimeListXml(string dates)
        {
            if (string.IsNullOrWhiteSpace(dates))
            {
                return null;
            }

            return XDocument.Parse(dates).Root.Elements()
                .Select(e => StringToRfc3339Date(e.Value).GetValueOrDefault())
                .ToList();
        }

        /// <summary>
        /// Converts given DateTime to XML string as per RFC 3339 time format.
        /// </summary>
        /// <param name="date">DateTime object.</param>
        /// <param name="rootName">Root name.</param>
        /// <returns>Date time as string.</returns>
        public static string ToRfc3339DateTimeXml(DateTime? date, string rootName = null)
        {
            return XmlPrinter.Print(date, Rfc3339DateToString, rootName ?? "DateTime");
        }

        /// <summary>
        /// Converts given DateTime data to XML string as per RFC 3339 time format.
        /// </summary>
        /// <param name="dates">Dates enumeration.</param>
        /// <param name="rootName">Root name.</param>
        /// <param name="arrayNodeName">Node name.</param>
        /// <param name="arrayItemName">Item name.</param>
        /// <returns>Date time as string.</returns>
        public static string ToRfc3339DateTimeListXml(IEnumerable<DateTime?> dates, string rootName = null, string arrayNodeName = null, string arrayItemName = null)
        {
            return XmlPrinter.PrintArray(dates, Rfc3339DateToString, rootName ?? "DateTime", arrayItemName ?? "dateTime", arrayNodeName);
        }
    }
}
