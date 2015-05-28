using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Common.Database
{
    public class WorldDatabase
    {
        protected Dictionary<ulong, Solar> solars { get; set; }
        protected Dictionary<ulong, Account> accounts { get; set; }
        protected Dictionary<ulong, CharacterPersonality> characters { get; set; }

        public WorldDatabase()
        {
            accounts = new Dictionary<ulong, Account>();
            solars = new Dictionary<ulong, Solar>();
            characters = new Dictionary<ulong, CharacterPersonality>();
        }
    }
}
