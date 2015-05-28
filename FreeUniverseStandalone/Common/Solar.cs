using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Common.Database;
using FreeUniverse.Core.Serialization;
using FreeUniverse.Core;
using FreeUniverse.World;
using FreeUniverse.Arch;

namespace FreeUniverse.Common
{
    [FastSerializable]
    public class SolarStructureInfo
    {
        public SolarStructureInfo()
        {

        }

        public SolarStructureInfo(INIReaderHeader header)
        {

        }

        public SolarStructureInfo(WorldObject obj)
        {

        }
    }

    // MH: solar ulong id is only valid inside CharacterPersonality
    // it may be non unique through all universe database
    [FastSerializable]
    public class Solar : DatabaseElement<Solar>
    {
        public CharacterPersonality owner { get; set; }

        [FastSerializable]
        public ulong ownerCharacter { get; set; }

        [FastSerializable]
        public uidkey ownerCharacterUID { get; set; }

        [FastSerializable]
        public SolarStructureInfo info { get; set; }
    }
}
