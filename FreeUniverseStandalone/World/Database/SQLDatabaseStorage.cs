using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using UnityEngine;
using FreeUniverse.Core;

namespace FreeUniverse.World.Database
{
    public class SQLDatabaseStorage<T> : IDatabaseStorage<T>
        where T : DatabaseStorageElement, new()
    {
        static string DATABASE_NAME = "free_universe_database";
        static string DATABASE_USER = "freeuniverse";
        static string DATABASE_PASSWORD = "freeuniverse";

        MySqlConnection _sqlConnection;
        string _table;

        public SQLDatabaseStorage(string table)
        {
            _table = table;
            string connectString = "Database=" + DATABASE_NAME + ";Data Source=127.0.0.1;User Id=" + DATABASE_USER + ";Password=" + DATABASE_PASSWORD;
            _sqlConnection = new MySqlConnection(connectString);
            _sqlConnection.Open();
        }

        ~SQLDatabaseStorage()
        {
            _sqlConnection.Close();
        }

        public int Insert(T element)
        {
            int result = 0;

            MySqlCommand cmd = new MySqlCommand("INSERT INTO " + DATABASE_NAME + "." + _table + "(id, data, mark) VALUES( @id, @data, '0' )", _sqlConnection);
            cmd.Parameters.AddWithValue("@id", element.id);
            cmd.Parameters.AddWithValue("@data", XmlSerializeHelpers.SerializeToXMLString(element));

            // Catch if duplicate entry found
            try
            {
                result += cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                return 0;
            }

            return result;
        }

        public int Insert(List<T> elements)
        {
            int result = 0;

            if (elements.Count == 0)
                return -1;

            foreach (T e in elements)
                result += Insert(e);

            return result;
        }

        public int Remove(ulong key)
        {
            int result = 0;

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _sqlConnection;
            cmd.CommandText = "delete from " + DATABASE_NAME + "." + _table + " where id=@i";
            cmd.Parameters.AddWithValue("@i", key);
            result += cmd.ExecuteNonQuery();

            return result;
        }

        public int Remove(List<ulong> keys)
        {
            int result = 0;

            foreach (ulong e in keys)
                result += Remove(e);

            return result;
        }

        public int Update(T element)
        {
            int result = 0;

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _sqlConnection;
            cmd.CommandText = "update " + DATABASE_NAME + "." + _table + " set data=@d where id=@i";
            cmd.Parameters.AddWithValue("@d", XmlSerializeHelpers.SerializeToXMLString(element));
            cmd.Parameters.AddWithValue("@i", element.id);
            result += cmd.ExecuteNonQuery();

            return result;
        }

        public int Update(List<T> elements)
        {
            int result = 0;

            foreach (T e in elements)
                result += Update(e);

            return result;
        }

        T _type = new T();

        public int Get(ulong key, out T element)
        {

            element = null;

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _sqlConnection;
            cmd.CommandText = "select data from " + DATABASE_NAME + "." + _table + " where id=@i";
            cmd.Parameters.AddWithValue("@i", key);

            string objString = (string)cmd.ExecuteScalar(); // data column should be of type string

            if ((null != objString) && (objString.Length > 0))
            {
                element = (T)XmlSerializeHelpers.DeserializeFromXMLString(objString, _type.GetType());
                return 1;
            }
  
            return 0;
        }

        public int Get(List<ulong> keys, ref Dictionary<ulong, T> elements)
        {
            int result = 0;

            foreach (ulong key in keys)
            {
                T eout = default(T);
                Get(key, out eout);

                if (null != eout)
                    result++;

                elements[key] = eout;
            }

            return result;
        }

        public int Mark(ulong key, uint value)
        {

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _sqlConnection;
            cmd.CommandText = "update table " + DATABASE_NAME + "." + _table + " set mark=@val where id=@i";
            cmd.Parameters.AddWithValue("@i", key);
            cmd.Parameters.AddWithValue("@val", value);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                return 0;
            }

            return 1;
        }

        public int GetMark(ulong key, out uint value)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _sqlConnection;
            cmd.CommandText = "select mark from " + DATABASE_NAME + "." + _table + " where id=@i";
            cmd.Parameters.AddWithValue("@i", key);

            byte[] stringData = (byte[])cmd.ExecuteScalar();

            string str = System.Text.Encoding.UTF8.GetString((byte[])cmd.ExecuteScalar());

            if (null != str && str.Length > 0)
            {
                value = UInt32.Parse(str);
                return 1;
            }

            value = 0;
            return 0;
        }

        public int SetParent(ulong key, ulong parent)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _sqlConnection;
            cmd.CommandText = "update table " + DATABASE_NAME + "." + _table + " set parent=@val where id=@i";
            cmd.Parameters.AddWithValue("@i", key);
            cmd.Parameters.AddWithValue("@val", parent);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                return 0;
            }

            return 1;
        }

        public int GetParent(ulong key, out ulong parent)
        {
            parent = 0;
            return 0;
        }

        public int GetByParent(ulong parent, ref Dictionary<ulong, T> elements)
        {

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _sqlConnection;
            cmd.CommandText = "select id, data from " + DATABASE_NAME + "." + _table + " where parent=@i";
            cmd.Parameters.AddWithValue("@i", parent);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                T element = (T)XmlSerializeHelpers.DeserializeFromXMLString(reader.GetString(1), _type.GetType());
                ulong key = reader.GetUInt64(0);
            }

            reader.Close();

            return 0;
        }
    }
}
