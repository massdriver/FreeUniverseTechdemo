using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FreeUniverse.Core
{
    public abstract class XmlSerializableObject : IXmlSerializable
    {
        public abstract void Write(XmlWriter writer);
        public abstract bool Read(XmlReader reader);

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            while (!reader.EOF) { if (!Read(reader)) reader.Read(); }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            Write(writer);
        }

        #endregion
    }
}
