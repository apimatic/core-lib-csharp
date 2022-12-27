using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace APIMatic.Core.Utilities.Date.Xml
{
    internal class XmlPrinter
    {
        internal static string Print<T>(T obj, Func<T, string> converter = null, string rootName = null)
        {
            if (obj == null)
            {
                return null;
            }
            var xml = new XmlPrinter();
            xml.StartDocument();
            xml.AddElement(rootName, converter(obj));
            xml.CloseDocument();
            return xml.ToString();
        }

        internal static string PrintArray<T>(IEnumerable<T> objList, Func<T, string> converter, string root = null, string itemName = null, string nodeName = null)
        {
            var xml = new XmlPrinter();
            xml.StartDocument();
            xml.StartItem(root);
            if (nodeName != null)
            {
                xml.StartItem(nodeName);
            }
            if (objList == null)
            {
                xml.AddElement(itemName, string.Empty);
            }
            else
            {
                objList.ToList().ForEach(obj => xml.AddElement(itemName, converter(obj)));
            }
            if (nodeName != null)
            {
                xml.CloseItem();
            }
            xml.CloseItem();
            xml.CloseDocument();
            return xml.ToString();
        }

        private readonly StringWriter stringWriter;
        internal readonly XmlWriter writer;
        internal XmlPrinter()
        {
            stringWriter = new StringWriter();
            writer = XmlWriter.Create(stringWriter);
        }

        internal void StartDocument()
        {
            writer.WriteStartDocument();
        }

        internal void StartItem(string rootName)
        {
            writer.WriteStartElement(rootName);
        }

        internal void AddElement(string itemName, string value)
        {
            writer.WriteElementString(itemName, value);
        }

        internal void CloseItem()
        {
            writer.WriteEndElement();
        }

        internal void CloseDocument()
        {
            writer.WriteEndDocument();
            writer.Close();
        }

        public override string ToString()
        {
            var xml = XElement.Parse(stringWriter.ToString());
            xml.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();
            return xml.ToString();
        }
    }
}
