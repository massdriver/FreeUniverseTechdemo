using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FreeUniverse.Core;

namespace FreeUniverse.World.Database
{
    public class DatabaseStorageElement : XmlSerializableObject
    {
        public DatabaseStorageElement()
        {

        }
        
        static string FIELD_ID = "id";
        ulong _id;
        public ulong id { get { return _id; } set { _id = value; } }

        override public void Write(XmlWriter writer)
        {
            XmlSerializeHelpers.SerializeUnsignedLong(writer, FIELD_ID, _id);
        }

        override public bool Read(XmlReader reader)
        {
            if (reader.IsStartElement(FIELD_ID))
            {
                XmlSerializeHelpers.DeserializeUnsignedLong(reader);
                return true;
            }

            return false;
        }
    }
}
