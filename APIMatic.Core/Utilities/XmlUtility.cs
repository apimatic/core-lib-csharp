// <copyright file="XmlUtility.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace APIMatic.Core.Utilities
{
    /// <summary>
    /// XmlUtility contains a bunch of utility methods.
    /// </summary>
    public static class XmlUtility
    {
        /// <summary>
        /// XML serialization of the given object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="rootName">The root name for xml.</param>
        /// <typeparam name="T">Type param.</typeparam>
        /// <returns>The XML serialized string.</returns>
        public static string ToXml<T>(T obj, string rootName = null)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var text = XmlWriter.Create(sb);
            var serializer = rootName != null ? new XmlSerializer(typeof(T), new XmlRootAttribute(rootName)) :
                new XmlSerializer(typeof(T));
            serializer.Serialize(text, obj);
            var xml = XElement.Parse(sb.ToString());
            xml.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();

            return xml.ToString();
        }

        /// <summary>
        /// XML serialization of a model type object to a XML array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="arrayName">The root name of the array element.</param>
        /// <param name="arrayItemName">The name of each item in the array.</param>
        /// <typeparam name="T">Type param.</typeparam>
        /// <returns>The XML serialized string.</returns>
        public static string ModelsArrayToXml<T>(List<T> obj, string arrayName = null, string arrayItemName = null)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            if (arrayItemName != null)
            {
                var sb = new StringBuilder();
                var xmlWriter = XmlWriter.Create(sb);
                var arrayItem = new XmlRootAttribute(arrayItemName);

                xmlWriter.WriteStartDocument();

                if (arrayName != null)
                {
                    xmlWriter.WriteStartElement(arrayName);
                }

                foreach (var item in obj as IEnumerable)
                {
                    var serializer = new XmlSerializer(typeof(T), arrayItem);
                    serializer.Serialize(xmlWriter, item);
                }
                if (arrayName != null)
                {
                    xmlWriter.WriteEndElement(); 
                }
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
                var xml = XElement.Parse(sb.ToString()); 
                xml.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();

                return xml.ToString();
            }
            else
            {
                return ToXml(obj, arrayName);
            }
        }

        /// <summary>
        /// XML serialization of native type object to a XML array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="arrayName">The root name of the array element.</param>
        /// <param name="arrayItemName">The name of each item in the array.</param>
        /// <typeparam name="T">Type param.</typeparam>
        /// <returns>The XML serialized string.</returns>
        public static string NativeTypesArrayToXml<T>(List<T> obj, string arrayName = null, string arrayItemName = null)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            var arrayNodeName = string.Empty;
            var xml = new StringBuilder();
            arrayNodeName = arrayName ?? typeof(T).Name;
            xml.Append($"<{arrayNodeName}>");
            foreach (var item in obj as IEnumerable)
            {
                GenerateSubArrayXML<T>(arrayItemName, xml, item);
            }
            xml.Append("</" + arrayNodeName + ">");
            var xmlDoc = XElement.Parse(xml.ToString());
            xmlDoc.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();

            return xmlDoc.ToString();
        }

        private static void GenerateSubArrayXML<T>(string arrayItemName, StringBuilder stringBuilder, object item)
        {    
            arrayItemName = arrayItemName ?? typeof(T).Name;
            stringBuilder.Append($"<{arrayItemName}>{item}</{arrayItemName}>");
        }

        /// <summary>
        /// XML deserialization of the given string into the given type.
        /// </summary>
        /// <param name="text">The string containing the xml.</param>
        /// <typeparam name="T">Type param.</typeparam>
        /// <param name="rootName">Root name.</param>
        /// <returns>An instance of the specified type.</returns>
        public static T FromXml<T>(string text, string rootName = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return default;
            }

            var serializer = rootName != null ? new XmlSerializer(typeof(T), new XmlRootAttribute(rootName)) :
                new XmlSerializer(typeof(T));
            var sr = new StringReader(text);
            var ex = (T)serializer.Deserialize(sr);

            return ex;
        }

        /// <summary>
        /// XML deserialization of a native type string.
        /// </summary>
        /// <param name="text">The string containing the xml.</param>
        /// <typeparam name="T">Type param.</typeparam>
        /// <returns>A list of native type.</returns>
        public static List<T> NativeTypesArrayFromXml<T>(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return default;
            }

            var mc = Regex.Matches(text, @">\w.*?</");
            var list = new List<T>();
            string val;

            foreach (Match m in mc)
            {
                val = m.Value.TrimStart('>').TrimEnd(new char[] { '<', '/' });
                list.Add((T)Convert.ChangeType(val, typeof(T)));
            }

            return list;
        }

        /// <summary>
        /// XML serialization of a simple dictionary.
        /// </summary>
        /// <param name="dict">The dictionary to be serialized.</param>
        /// <param name="rootName">The rootName of dictionary in the xml.</param>
        /// <typeparam name="T">Type param.</typeparam>
        /// <returns>A string containing the xml.</returns>
        public static string DictionaryToXml<T>(Dictionary<string, T> dict, string rootName)
        {
            var element = new XElement(rootName);

            if (dict == null)
            {
                return null;
            }

            foreach (var entry in dict)
            {
                element.Add(new XElement("entry", new XAttribute("key", entry.Key), entry.Value));
            }

            return element.ToString();
        }

        /// <summary>
        /// XML deserialization of a simple dictionary.
        /// </summary>
        /// <param name="xml">The string containing the xml.</param>
        /// <typeparam name="T">Type param.</typeparam>
        /// <returns>A dictionary of string and value of the specified type.</returns>
        public static Dictionary<string, T> XmlToDictionary<T>(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return null;
            }

            var element = XElement.Parse(xml);
            var dict = new Dictionary<string, T>();

            foreach (var e in element.Elements())
            {
                dict.Add(e.FirstAttribute.Value, (T)Convert.ChangeType(e.Value, typeof(T)));
            }

            return dict;
        }
    }
}
