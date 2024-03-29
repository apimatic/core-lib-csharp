﻿// <copyright file="UnixDateTimeXmlUtility.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace APIMatic.Core.Utilities.Date.Xml
{
    /// <summary>
    /// UnixDateTimeXmlConverter contains a bunch of utility methods.
    /// </summary>
    public class UnixDateTimeXmlConverter
    {
        /// <summary>
        /// Converts given date string to unix datetime.
        /// </summary>
        /// <param name="date">Date string.</param>
        /// <returns>DateTime object.</returns>
        public static DateTime? StringToUnixDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            DateTime unixDateTime;
            unixDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(date)).DateTime.ToLocalTime();

            return unixDateTime;
        }

        /// <summary>
        /// Converts given DateTime to unix datetime string.
        /// </summary>
        /// <param name="date"> DateTime object.</param>
        /// <returns>Date time string.</returns>
        public static string UnixDateToString(DateTime? date)
        {
            if (date == null)
            {
                return null;
            }
            var dateTimeOffset = new DateTimeOffset(date.Value.ToUniversalTime());
            return dateTimeOffset.ToUnixTimeSeconds().ToString();
        }

        /// <summary>
        /// Converts given XML string to unix datetime.
        /// </summary>
        /// <param name="date">Date time string.</param>
        /// <returns>DateTime object.</returns>
        public static DateTime? FromUnixDateTimeXml(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            return StringToUnixDate(XDocument.Parse(date).Root.Value);
        }

        /// <summary>
        /// Extracts DateTime list in unix datetime from the given XML string.
        /// </summary>
        /// <param name="dates">Dates as string.</param>
        /// <returns>List of DateTime objects.</returns>
        public static List<DateTime> FromUnixDateTimeListXml(string dates)
        {
            if (string.IsNullOrWhiteSpace(dates))
            {
                return null;
            }

            return XDocument.Parse(dates).Root.Elements()
                .Select(e => StringToUnixDate(e.Value).GetValueOrDefault())
                .ToList();
        }

        /// <summary>
        /// Converts given DateTime to unix datetime and returns it as XML string.
        /// </summary>
        /// <param name="date">DateTime object.</param>
        /// <param name="rootName">Root name.</param>
        /// <returns>Date time string.</returns>
        public static string ToUnixDateTimeXml(DateTime? date, string rootName = null)
        {
            return XmlPrinter.Print(date, UnixDateToString, rootName ?? "DateTime");
        }

        /// <summary>
        /// Converts given DateTime data to unix datetime XML string.
        /// </summary>
        /// <param name="dates">DateTime enumeration.</param>
        /// <param name="rootName">Root name.</param>
        /// <param name="arrayNodeName">Node name.</param>
        /// <param name="arrayItemName">Item name.</param>
        /// <returns>DateTime as string.</returns>
        public static string ToUnixDateTimeListXml(IEnumerable<DateTime?> dates, string rootName = null, string arrayNodeName = null, string arrayItemName = null)
        {
            return XmlPrinter.PrintArray(dates, UnixDateToString, rootName ?? "DateTime", arrayItemName ?? "dateTime", arrayNodeName);
        }
    }
}
