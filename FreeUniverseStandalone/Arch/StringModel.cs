using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Core;

namespace FreeUniverse.Arch
{
    [Deprecated]
    public class StringModel
    {
        public static string STRING_LIST_FILE_PATH = "Archdata/strings_list";

        public void LoadStrings()
        {
            LoadStringFromUnityAsset(STRING_LIST_FILE_PATH);
        }

        void LoadStringFromUnityAsset(string assetFile)
        {
            TextAsset txt = (TextAsset)Resources.Load(assetFile);

            if (txt == null)
            {
                Debug.Log("StringModel: failed to locate file - " + assetFile);
                return;
            }

            // now loop through all strings
            string[] fileNames = txt.text.Split('\n');

            foreach (string str in fileNames)
            {

                if (str.StartsWith("//"))
                    continue;

                if (str.Length == 0)
                    continue;

                // Load 
                {
                    TextAsset iniText = (TextAsset)Resources.Load(str);
                }
            }
        }

        Dictionary<int, string> _strings = new Dictionary<int,string>();

        public string GetString(int id)
        {
            string s = null;
            _strings.TryGetValue(id, out s);
            return s;
        }
    }
}
