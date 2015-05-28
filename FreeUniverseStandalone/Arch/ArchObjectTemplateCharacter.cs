using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World.Database;
using FreeUniverse.Common;

namespace FreeUniverse.Arch
{
    public class ArchObjectTemplateCharacter : ArchObject
    {
        ulong[] _initialCredits = new ulong[CreditAccount.MAX_CREDIT_TYPES];

        ulong _faction = 0;
        public ulong faction { get { return _faction; } }

        ulong _system = 0;
        public ulong system { get { return _system; } }

        ulong _dockedSolarGUID = 0;
        public ulong dockedSolar { get { return _dockedSolarGUID; } }

        //CharacterFactionReputation _factionReputation = new CharacterFactionReputation();
        //public CharacterFactionReputation factionReputation { get { return _factionReputation; } }

        public override void ReadHeader(INIReaderHeader header)
        {
            base.ReadHeader(header);
        }

        public override void ReadParameter(INIReaderParameter parameter)
        {
            base.ReadParameter(parameter);
        }
    }
}
