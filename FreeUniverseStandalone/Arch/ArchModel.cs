using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Core;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FreeUniverse.Arch
{
    
    public class ArchModel
    {
        private static IndexArchStorage<ArchObjectSystem> archSystems { get; set; }
        private static IndexArchStorage<ArchWorldObjectComponent> archComponents { get; set; }
        private static IndexArchStorage<ArchZone> archZones { get; set; }
        private static IndexArchStorage<ArchCharacterTemplate> archCharacterTemplates { get; set; }
        private static IndexArchStorage<ArchGenericFaction> archFactions { get; set; }
        private static IndexArchStorage<ArchObjectBaseContents> archBases { get; set; }
        private static string[] archStrings { get; set; }

        public const int MAX_STRINGS = 4096;

        public const string COMPONENTS_DIR = "Archdata/Components";
        public const string SYSTEMS_DIR = "Archdata/Systems";
        public const string STRINGS_DIR = "Archdata/Strings";
        public const string ZONES_DIR = "Archdata/Zones";
        public const string CHARACTER_TEMPLATES_DIR = "Archdata/CharacterTemplates";
        public const string BASES_DIR = "Archdata/Bases";

        public static void Init()
        {
            archFactions = new IndexArchStorage<ArchGenericFaction>();
            archCharacterTemplates = new IndexArchStorage<ArchCharacterTemplate>();
            archZones = new IndexArchStorage<ArchZone>();
            archComponents = new IndexArchStorage<ArchWorldObjectComponent>();
            archSystems = new IndexArchStorage<ArchObjectSystem>();
            archBases = new IndexArchStorage<ArchObjectBaseContents>();
            archStrings = new string[MAX_STRINGS];

            Debug.Log("ArchModel: Loading arch data");

            // strings should be loaded first to resolve ids inside other ArchObjects on load
            Debug.Log("ArchModel: strings=" + LoadAllStrings(archStrings, STRINGS_DIR));

            // Bases before systems
            Debug.Log("ArchModel: bases=" + UnityTextAssetArchLoader<ArchObjectBaseContents>.LoadAll(archBases, BASES_DIR, "Base"));

            Debug.Log("ArchModel: templates=" + UnityTextAssetArchLoader<ArchCharacterTemplate>.LoadAll(archCharacterTemplates, CHARACTER_TEMPLATES_DIR, "Character"));
            Debug.Log("ArchModel: systems=" + UnityTextAssetArchLoader<ArchObjectSystem>.LoadAll(archSystems, SYSTEMS_DIR, "System"));
            Debug.Log("ArchModel: components=" + UnityTextAssetArchLoader<ArchWorldObjectComponent>.LoadAll(archComponents, COMPONENTS_DIR, "Component"));
            Debug.Log("ArchModel: zones=" + UnityTextAssetArchLoader<ArchZone>.LoadAll(archZones, ZONES_DIR, "Zone"));
            

            InitTemplates();
        }

        public static ArchObjectBaseContents GetBase(ulong id)
        {
            return archBases.GetByKey(id);
        }

        public static string GetString(int id)
        {
            return archStrings[id];
        }

        public static ArchWorldObjectComponent GetComponent(int index)
        {
            return archComponents.GetByIndex(index);
        }

        public static ArchWorldObjectComponent GetComponent(ulong id)
        {
            return archComponents.GetByKey(id);
        }

        public static ArchWorldObjectComponent GetComponent(string nickname)
        {
            return GetComponent(HashUtils.StringToUINT64(nickname));
        }

        public static ArchObjectSystem GetSystem(ulong id)
        {
            return archSystems.GetByKey(id);
        }

        public static ArchZone GetZone(ulong id)
        {
            return archZones.GetByKey(id);
        }

        public static ArchCharacterTemplate GetCharTemplate(ulong id)
        {
            return archCharacterTemplates.GetByKey(id);
        }

        public static void InitTemplates()
        {
            PlayerStartTemplate.templates = new Dictionary<ulong, PlayerStartTemplate>();

            {
                PlayerStartTemplate rha = new PlayerStartTemplate("Red Hessian Wraith", "rha_vhf_hull", new Vector3(0.0f, 0.0f, -10000.0f));
                PlayerStartTemplate.templates.Add(rha.id, rha);
            }

            {
                PlayerStartTemplate civ = new PlayerStartTemplate("Raven Talon", "civ_vhf_hull", new Vector3(0.0f, 0.0f, -10000.0f));
                PlayerStartTemplate.templates.Add(civ.id, civ);
            }

        }

        public static int LoadAllStrings(string[] outStrings, string dirPath)
        {
            UnityEngine.Object[] assets = Resources.LoadAll(dirPath, typeof(TextAsset));

            int loaded = 0;

            foreach (UnityEngine.Object e in assets)
            {
                TextAsset txtAsset = e as TextAsset;

                if (e == null) continue;

                loaded += TextStringReader.LoadStrings(outStrings, txtAsset.text);
            }

            return loaded;
        }

        public static void Write(string file)
        {
            /*
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);

            formatter.Serialize(stream, archSystems);
            formatter.Serialize(stream, archComponents);
            formatter.Serialize(stream, archZones);
            formatter.Serialize(stream, archCharacterTemplates);
            formatter.Serialize(stream, archFactions);
            formatter.Serialize(stream, archBases);
            formatter.Serialize(stream, archStrings);

            stream.Close();
            */
        }

        public static void Read(string file)
        {

        }
    }
}
