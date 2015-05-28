using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using FreeUniverse.Core.Serialization;

namespace FreeUniverse.Common.Database
{
    public class MySqlDatabase<T>
    {
        protected FastSerializer serializer { get; set; }
        protected MySqlConnection sqlConnection { get; set; }

        public MySqlDatabase(MySqlConnection connection)
        {
            sqlConnection = connection;
            serializer = new FastSerializer(typeof(T));
        }
    }
}
