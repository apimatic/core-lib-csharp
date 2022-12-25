// <copyright file="UnixDateTimeXmlUtility.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
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
        public static string UnixDateToString(DateTime date)
        {
            var dateTimeOffset = new DateTimeOffset(date.ToUniversalTime());
            var unixDateTime = dateTimeOffset.ToUnixTimeSeconds().ToString();

            return unixDateTime;
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

            using (var reader = XmlReader.Create(new StringReader(date)))
            {
                reader.Read();
                var ele = reader.ReadElementContentAsString();

                if (string.IsNullOrWhiteSpace(ele))
                {
                    return null;
                }

                var dateTime = StringToUnixDate(ele);
                return dateTime;
            }
        }

        /// <summary>
        /// Converts given DateTime to unix datetime and returns it as XML string.
        /// </summary>
        /// <param name="date">DateTime object.</param>
        /// <param name="rootName">Root name.</param>
        /// <returns>Date time string.</returns>
        public static string ToUnixDateTimeXml(DateTime date, string rootName = null)
        {
            using (var sb = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sb))
                {
                    var nodeName = rootName ?? "DateTime";
                    var dateTime = UnixDateToString(date);
                    writer.WriteStartDocument();
                    writer.WriteElementString(nodeName, dateTime);
                    writer.WriteEndDocument();
                }

                var xml = XElement.Parse(sb.ToString());
                xml.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();

                return xml.ToString();
            }
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

            var doc = XDocument.Parse(dates);
            var list = doc.Root.Elements()
                .Select(e => StringToUnixDate(e.Value).GetValueOrDefault()).ToList();

            return list;
        }

        /// <summary>
        /// Converts given DateTime data to unix datetime XML string.
        /// </summary>
        /// <param name="dates">DateTime enumeration.</param>
        /// <param name="rootName">Root name.</param>
        /// <param name="arrayNodeName">Node name.</param>
        /// <param name="arrayItemName">Item name.</param>
        /// <returns>DateTime as string.</returns>
        public static string ToUnixDateTimeListXml(IEnumerable<DateTime> dates, string rootName = null, string arrayNodeName = null, string arrayItemName = null)
        {
            using (var sb = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sb))
                {
                    var root = rootName ?? "DateTime";
                    var itemName = arrayItemName ?? "dateTime";

                    writer.WriteStartDocument();
                    writer.WriteStartElement(root);

                    if (arrayNodeName != null)
                    {
                        writer.WriteStartElement(arrayNodeName);
                    }

                    if (dates == null)
                    {
                        writer.WriteElementString(itemName, string.Empty);
                    }
                    else
                    {
                        foreach (var date in dates)
                        {
                            var dateTime = UnixDateToString(date);
                            writer.WriteElementString(itemName, dateTime);
                        }
                    }

                    if (arrayNodeName != null)
                    {
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                var xml = XElement.Parse(sb.ToString());
                xml.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();

                return xml.ToString();
            }
        }
    }
}
