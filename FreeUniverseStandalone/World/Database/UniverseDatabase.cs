using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.World.Database
{
    public class UniverseDatabase
    {
        protected Dictionary<ulong,DatabaseStorageElementAccount> _accounts = new Dictionary<ulong,DatabaseStorageElementAccount>();
        protected Dictionary<ulong,DatabaseStorageElementCharacter> _characters = new Dictionary<ulong,DatabaseStorageElementCharacter>();
        protected Dictionary<ulong,DatabaseStorageElementSolar> _solars = new Dictionary<ulong,DatabaseStorageElementSolar>();
        
        public UniverseDatabase()
        {
            
        }
    }
}
