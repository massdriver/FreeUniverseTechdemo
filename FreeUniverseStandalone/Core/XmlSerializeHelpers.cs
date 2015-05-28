using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;

namespace FreeUniverse.Core
{
    public class XmlSerializeHelpers
    {
        public static readonly string ATTRIBUTE_TYPE = "t";
        public static readonly string ATTRIBUTE_KEY = "key";
        public static readonly string ATTIBUTE_TYPE_DICTIONARY_UINT_INT = "dui";
        public static readonly string ATTIBUTE_TYPE_DICTIONARY_LONG_OBJECT = "dui";
        public static readonly string ATTIBUTE_TYPE_STRING = "s";
        public static readonly string NODE_DICTIONARY_ELEMENT = "p";
        public static readonly string NODE_DICTIONARY_SIZE = "size";

        public static void SerializeInt(XmlWriter writer, string name, int value)
        {
            writer.WriteStartElement(name);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static int DeserializeInt(XmlReader reader, string name)
        {
            int i = 0;
            reader.ReadStartElement();
            i = reader.ReadContentAsInt();
            reader.ReadEndElement();
            return i;
        }

        public static void SerializeLong(XmlWriter writer, string name, long value)
        {
            writer.WriteStartElement(name);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static long DeserializeLong(XmlReader reader)
        {
            long i = 0;
            reader.ReadStartElement();
            i = reader.ReadContentAsLong();
            reader.ReadEndElement();
            return i;
        }

        public static void SerializeUnsignedLong(XmlWriter writer, string name, ulong value)
        {
            writer.WriteStartElement(name);
            writer.WriteValue(value.ToString());
            writer.WriteEndElement();
        }

        public static ulong DeserializeUnsignedLong(XmlReader reader)
        {
            ulong i = 0;
            reader.ReadStartElement();
            i = ulong.Parse(reader.ReadContentAsString());
            reader.ReadEndElement();
            return i;
        }

        public static void SerializeDictionary(XmlWriter writer, Dictionary<uint, int> dict, string name)
        {
            writer.WriteStartElement(name);
            SerializeInt(writer, NODE_DICTIONARY_SIZE, dict.Count);

            foreach (var kp in dict)
            {
                writer.WriteStartElement(NODE_DICTIONARY_ELEMENT);
                writer.WriteAttributeString(ATTRIBUTE_KEY, kp.Key.ToString());
                writer.WriteValue(kp.Value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public static Dictionary<uint, int> DeserializeDictionary(XmlReader reader)
        {
            Dictionary<uint, int> dict = new Dictionary<uint, int>();

            reader.ReadStartElement();

            int count = DeserializeInt(reader, NODE_DICTIONARY_SIZE);

            for (int i = 0; i < count; i++)
            {
                reader.MoveToElement();
                uint key = UInt32.Parse(reader.GetAttribute(ATTRIBUTE_KEY));

                reader.ReadStartElement();
                int value = reader.ReadContentAsInt();
                reader.ReadEndElement();

                dict[key] = value;
            }

            reader.ReadEndElement();

            return dict;
        }

        public static void SerializeString(XmlWriter writer, string name, string value)
        {
            writer.WriteStartElement(name);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static string DeserializeString(XmlReader reader)
        {
            string strOut;
            reader.ReadStartElement();
            strOut = reader.ReadString();
            reader.ReadEndElement();
            return strOut;
        }

        public static string SerializeToXMLString(IXmlSerializable obj)
        {
            XmlSerializer s = new XmlSerializer(obj.GetType());
            StringWriter str = new StringWriter();
            s.Serialize(str, obj);
            return str.ToString();
        }

        public static object DeserializeFromXMLString(string str, Type t)
        {
            XmlSerializer s = new XmlSerializer(t);
            return s.Deserialize(new StringReader(str));
        }
    }
}
