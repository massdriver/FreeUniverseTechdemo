using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Common
{
    public class FactionReputationStandings
    {

    }

    public class Faction
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public FactionReputationStandings reputation { get; set; }
        public bool userFaction { get; set; }
    }
}
