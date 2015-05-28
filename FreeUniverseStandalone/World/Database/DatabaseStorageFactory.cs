using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.World.Database
{
    public class DatabaseStorageFactory<T> where T : DatabaseStorageElement, new()
    {
        public static IDatabaseStorage<T> Create(string table)
        {
            return new SQLDatabaseStorage<T>(table);
        }
    }
}
