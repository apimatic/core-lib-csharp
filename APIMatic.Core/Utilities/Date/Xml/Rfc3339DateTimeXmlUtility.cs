// <copyright file="Rfc3339DateTimeXmlUtility.cs" company="APIMatic">
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
    /// CoreRfc3339DateTimeXmlUtility contains a bunch of utility methods.
    /// </summary>
    public class Rfc3339DateTimeXmlUtility
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

            using (var reader = XmlReader.Create(new StringReader(date)))
            {
                reader.Read();
                var ele = reader.ReadElementContentAsString();

                if (string.IsNullOrWhiteSpace(ele))
                {
                    return null;
                }

                var dateTime = StringToRfc3339Date(ele);
                return dateTime;
            }
        }

        /// <summary>
        /// Converts given DateTime to XML string as per RFC 3339 time format.
        /// </summary>
        /// <param name="date">DateTime object.</param>
        /// <param name="rootName">Root name.</param>
        /// <returns>Date time as string.</returns>
        public static string ToRfc3339DateTimeXml(DateTime? date, string rootName = null)
        {
            using (var sb = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sb))
                {
                    var nodeName = rootName ?? "DateTime";
                    var dateTime = Rfc3339DateToString(date);
                    if (!string.IsNullOrWhiteSpace(dateTime))
                    {
                        writer.WriteStartDocument();
                        writer.WriteElementString(nodeName, dateTime);
                        writer.WriteEndDocument();
                    }
                    else
                    {
                        return null;
                    }
                }

                var xml = XElement.Parse(sb.ToString());
                xml.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();

                return xml.ToString();
            }
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

            var doc = XDocument.Parse(dates);
            var list = doc.Root.Elements()
                .Select(e => StringToRfc3339Date(e.Value).GetValueOrDefault()).ToList();

            return list;
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
                            var dateTime = Rfc3339DateToString(date);
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
