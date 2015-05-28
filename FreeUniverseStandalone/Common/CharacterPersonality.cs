using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using FreeUniverse.Common.Database;
using FreeUniverse.Core.Serialization;
using FreeUniverse.Core;

namespace FreeUniverse.Common
{
    public class CharacterCreateDesc
    {
        public string name { get; set; }
        public string description { get; set; }
        public ulong template { get; set; }
        public ArchCharacterTemplate charTemplate { get; set; }

        public CharacterCreateDesc(string name, ulong template)
        {
            this.name = name;
            this.template = template;
            //charTemplate = ArchModel.Get
        }
    }

    public class CharacterAppearance
    {

    }

    public class ArchCharacterSkill : ArchObject
    {
        public List<ulong> requiredSkills { get; set; }
        public ulong trainingPoints { get; set; }
    }

    public class CharacterSkillEntry
    {
        public ulong skill { get; set; }
        public uint level { get; set; }
        public ulong trainingPoints { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public bool trainingInProgress { get; set; }

        public static ulong TimeIntervalToPoints(DateTime start, DateTime end)
        {
            return (ulong)(end - start).TotalSeconds;
        }
    }

    [FastSerializable]
    public class CharacterSkillSheet
    {
        [FastSerializable]
        public List<CharacterSkillEntry> queue { get; set; }

        [FastSerializable]
        public Dictionary<ulong, CharacterSkillEntry> skills { get; set; }

        public CharacterSkillBonusProvider skillBonusProvider { get; set; }

        public CharacterSkillSheet()
        {
            queue = new List<CharacterSkillEntry>();
            skills = new Dictionary<ulong, CharacterSkillEntry>();
            skillBonusProvider = new CharacterSkillBonusProvider(this);
        }
    }

    public enum SkillBonusType
    {
        Null,
        Unknown
    }

    public class CharacterSkillBonusProvider
    {
        const int MAX_SKILLS = 32;

        float[] bonusValues = new float[MAX_SKILLS];

        public float this[SkillBonusType type]
        {
            get
            {
                int id = (int)type;
                if (id < 0 || id >= bonusValues.Length) return 0.0f;
                return bonusValues[(int)type];
            }
        }

        private CharacterSkillSheet skillSheet { get; set; }

        public CharacterSkillBonusProvider(CharacterSkillSheet sheet)
        {
            skillSheet = sheet;
        }

        void Update()
        {

        }
    }

    public enum ReputationType
    {
        Hostile,
        Bad,
        Neutral,
        Friendly,
        Good
    }

    [FastSerializable]
    public class ReputationStandings
    {
        [FastSerializable]
        private Dictionary<ulong, float> reputation { get; set; }

        public ReputationStandings()
        {
            reputation = new Dictionary<ulong, float>();
        }

        public float this[ulong id]
        {
            get
            {
                float rep = 0.0f;
                reputation.TryGetValue(id, out rep);
                return rep;
            }
        }

        public ReputationType GetReputation(ulong faction)
        {
            float rep = this[faction];

            if (rep <= REP_HOSTILE) return ReputationType.Hostile;
            if (rep > REP_HOSTILE && rep <= REP_BAD) return ReputationType.Bad;

            if (rep >= REP_FRIENDLY) return ReputationType.Friendly;
            if (rep < REP_FRIENDLY && rep >= REP_GOOD) return ReputationType.Good;

            return ReputationType.Neutral;
        }

        public const float REP_HOSTILE = -0.8f;
        public const float REP_BAD = -0.4f;
        public const float REP_GOOD = 0.4f;
        public const float REP_FRIENDLY = 0.8f;
    }

    [FastSerializable]
    public class CharacterLocation
    {
        // Static dock, valid through all universe
        [FastSerializable]
        public ulong staticSystem { get; set; }

        [FastSerializable]
        public ulong staticBase { get; set; }

        // Procedural systems and bases, those ids are valid only through zone server
        [FastSerializable]
        public ulong dynamicSystem { get; set; }

        [FastSerializable]
        public ulong dynamicBase { get; set; }

        // User type dock, valid only if uidkey of character is same as in characterHost
        // if not, character should be treated as dead or moved to origin base
        [FastSerializable]
        public uidkey characterUniqueKey { get; set; }

        // Char id inside mysql database
        [FastSerializable]
        public ulong characterHost { get; set; }

        // solar id inside character personality
        [FastSerializable]
        public ulong characterHostSolar { get; set; }

        public void ReadHeader(INIReaderHeader header)
        {
            foreach (INIReaderParameter p in header.parameters)
            {
                if (p.Check("static_base"))
                    staticBase = p.GetStrkey64(0);
                else if (p.Check("static_system"))
                    staticSystem = p.GetStrkey64(0);
            }
        }
    }

    [FastSerializable]
    public class CharacterPersonality : DatabaseElement<CharacterPersonality>
    {
        [FastSerializable]
        public DateTime creationDate { get; set; }

        [FastSerializable]
        public string description { get; set; }

        [FastSerializable]
        public string firstname { get; set; }

        [FastSerializable]
        public string lastname { get; set; }

        [FastSerializable]
        public ReputationStandings reputation { get; set; }

        [FastSerializable]
        public CharacterSkillSheet skillSheet { get; set; }

        [FastSerializable]
        public CreditAccount creditAccount { get; set; }

        [FastSerializable]
        public Dictionary<ulong, Solar> privateSolarObjects { get; set; }

        [FastSerializable]
        public uidkey uniqueKey { get; set; }

        [FastSerializable]
        public CharacterLocation location { get; set; }

        public CharacterPersonality()
        {
            skillSheet = new CharacterSkillSheet();
            creditAccount = new CreditAccount();
            reputation = new ReputationStandings();
            uniqueKey = new uidkey(null);
            location = new CharacterLocation();
        }

        public CharacterPersonality(CharacterCreateDesc desc)
            : base()
        {
            uniqueKey = new uidkey(desc.name + DateTime.Now.ToString());
        }
    }

    /*
     * Skill list:
     *
     * "Faction" LF/HF/VHF/Trans/GB/Cruiser/BS/Dread operation
     * Required to operate ship type
     */
}
