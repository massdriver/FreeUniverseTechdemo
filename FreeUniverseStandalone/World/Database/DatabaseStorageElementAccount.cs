using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Core;

namespace FreeUniverse.World.Database
{
    public class DatabaseStorageElementAccount : DatabaseStorageElement
    {
        string _user = ""; // to avoid null values being stored to xml
        string _password = ""; // to avoid null values being stored to xml
        DateTime _creationDate = DateTime.MaxValue;
        DateTime _lastLoginDate = DateTime.MaxValue;
        DateTime _banUntilDate = DateTime.MaxValue;
        List<ulong> _characterKeys = new List<ulong>(); // stored
        List<ulong> _solarKeys = new List<ulong>(); // stored

        // just for reference purposes while account is loaded
        Dictionary<ulong, DatabaseStorageElementCharacter> _characters = new Dictionary<ulong, DatabaseStorageElementCharacter>();
        Dictionary<ulong, DatabaseStorageElementSolar> _solars = new Dictionary<ulong, DatabaseStorageElementSolar>();

        bool _writePassword = true;
        public bool writePassword { get { return _writePassword; } set { _writePassword = value; } }

        public string user { get { return _user; } set { _user = value; } }
        public string password { get { return _password; } set { _password = value; } }

        public static readonly string FIELD_USER = "user";
        public static readonly string FIELD_PASSWORD = "password";

        public override bool Read(System.Xml.XmlReader reader)
        {
            if (reader.IsStartElement(FIELD_USER))
            {
                _user = XmlSerializeHelpers.DeserializeString(reader);
                return true;
            }
            else if (reader.IsStartElement(FIELD_PASSWORD))
            {
                _password = XmlSerializeHelpers.DeserializeString(reader);
                return true;
            }

            return base.Read(reader);
        }

        public override void Write(System.Xml.XmlWriter writer)
        {
            base.Write(writer);

            XmlSerializeHelpers.SerializeString(writer, FIELD_USER, _user);

            if( _writePassword ) XmlSerializeHelpers.SerializeString(writer, FIELD_PASSWORD, _password);
            else XmlSerializeHelpers.SerializeString(writer, FIELD_PASSWORD, "");
        }
    }
}
