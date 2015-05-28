using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using UnityEngine;
using FreeUniverse.Core;
using FreeUniverse.Core.Serialization;

namespace FreeUniverse.Common.Database
{
    public class SQLDatabaseStorage<T> : IDatabaseStorage<T>
        where T : DatabaseElement<T>, new()
    {
        private FastSerializer serializer { get; set; }
        private MySqlConnection sqlConnection { get; set; }
        private string tableName { get; set; }

        public SQLDatabaseStorage(string table, MySqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
            serializer = new FastSerializer(typeof(T));

            tableName = table;
        }

        public int Insert(T element)
        {
            int result = 0;

            MySqlCommand cmd = new MySqlCommand("insert into " + ConstData.DATABASE_NAME + "." + tableName + "(id, data, mark) values( @id, @data, '0' )", sqlConnection);
            cmd.Parameters.AddWithValue("@id", element.id);
            cmd.Parameters.AddWithValue("@data", serializer.Serialize(element));
            
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
            cmd.Connection = sqlConnection;
            cmd.CommandText = "delete from " + ConstData.DATABASE_NAME + "." + tableName + " where id=@i";
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
            cmd.Connection = sqlConnection;
            cmd.CommandText = "update " + ConstData.DATABASE_NAME + "." + tableName + " set data=@d where id=@i";
            cmd.Parameters.AddWithValue("@d", serializer.Serialize(element));
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

        public int Get(ulong key, out T element)
        {
            element = null;

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandText = "select data from " + ConstData.DATABASE_NAME + "." + tableName + " where id=@i";
            cmd.Parameters.AddWithValue("@i", key);

            string objString = (string)cmd.ExecuteScalar(); // data column should be of type string

            if ((null != objString) && (objString.Length > 0))
            {
                element = serializer.Deserialize(objString) as T;
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
            // update table free_universe_database.accounts set mark=1 where id=actual_account_id_that_is_sent_to_email
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandText = "update table " + ConstData.DATABASE_NAME + "." + tableName + " set mark=@val where id=@i";
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
            cmd.Connection = sqlConnection;
            cmd.CommandText = "select mark from " + ConstData.DATABASE_NAME + "." + tableName + " where id=@i";
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
            cmd.Connection = sqlConnection;
            cmd.CommandText = "update table " + ConstData.DATABASE_NAME + "." + tableName + " set parent=@val where id=@i";
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
            throw new Exception("unsupported yet");
        }

        public int GetByParent(ulong parent, ref Dictionary<ulong, T> elements)
        {
            throw new Exception("unsupported yet");

            /*
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandText = "select id, data from " + DATABASE_NAME + "." + tableName + " where parent=@i";
            cmd.Parameters.AddWithValue("@i", parent);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                T element = serializer.Deserialize(reader.GetString(1)) as T;
                //T element = (T)XmlSerializeHelpers.DeserializeFromXMLString(reader.GetString(1), _type.GetType());
                ulong key = reader.GetUInt64(0);
            }

            reader.Close();

            return 0;
            */

        }
    }
}
