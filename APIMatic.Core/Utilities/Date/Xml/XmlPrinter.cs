using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace APIMatic.Core.Utilities.Date.Xml
{
    internal class XmlPrinter
    {
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
