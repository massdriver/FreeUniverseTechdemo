using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Common;
using FreeUniverse.Net;
using FreeUniverse.Core;
using FreeUniverse.Core.Serialization;

namespace FreeUniverse.Arch
{
    [FastSerializable]
    public class ArchCharacterTemplate : ArchObject
    {
        [FastSerializable]
        public CharacterLocation startLocation { get; set; }
        [FastSerializable]
        public CharacterSkillSheet skillSheet { get; set; }
        [FastSerializable]
        public CreditAccount creditAccount { get; set; }
        [FastSerializable]
        public ReputationStandings reputation { get; set; }

        [FastSerializable]
        public List<Solar> solars { get; set; }

        public ArchCharacterTemplate()
        {
            creditAccount = new CreditAccount();
            reputation = new ReputationStandings();
            solars = new List<Solar>();
            skillSheet = new CharacterSkillSheet();
            startLocation = new CharacterLocation();
        }

        public override void ReadHeader(INIReaderHeader header)
        {
            base.ReadHeader(header);

            if (header.Check("CreditAccount"))
                creditAccount.ReadHeader(header);
            else if (header.Check("StartLocation"))
                startLocation.ReadHeader(header);
        }

    }
}
