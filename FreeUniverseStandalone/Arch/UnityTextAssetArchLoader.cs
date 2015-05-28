using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace FreeUniverse.Arch
{
    public class UnityTextAssetArchLoader<T>
            where T : ArchObject, new()
    {

        

        public static int LoadAll(IndexArchStorage<T> outVal, string dirPath, string masterSection)
        {
            UnityEngine.Object[] assets = Resources.LoadAll(dirPath, typeof(TextAsset));

            int loaded = outVal.Count;
            
            foreach (UnityEngine.Object e in assets)
            {
                TextAsset txtAsset = e as TextAsset;

                if (e == null) continue;

                INIReader reader = new INIReader(txtAsset.text);

                T archObj = null;

                foreach (INIReaderHeader h in reader.GetHeaders())
                {
                    if ((h.nickname.CompareTo(masterSection) == 0) && (archObj == null))
                    {
                        archObj = new T();
                        archObj.ReadHeader(h);
                    }
                    else if ((h.nickname.CompareTo(masterSection) != 0) && (archObj != null))
                    {
                        archObj.ReadHeader(h);
                    }
                    else if ((h.nickname.CompareTo(masterSection) == 0) && (archObj != null))
                    {
                        // Store previous
                        if( archObj.id != 0 )
                            outVal.Add(archObj);

                        // Create new and read current section
                        archObj = new T();
                        archObj.ReadHeader(h);
                    }
                }

                if (archObj != null && archObj.id != 0)
                    outVal.Add(archObj);

                
            }

            outVal.BuildIndexData();

            return outVal.Count - loaded;
        }

        public static void Load(IndexArchStorage<T> outVal, string assetFile, string masterSection)
        {
            TextAsset txt = (TextAsset)Resources.Load(assetFile);

            if (txt == null)
            {
                Debug.Log("ArchLoader: failed to locate file - " + assetFile);
                return;
            }

            // now loop through all strings
            StringReader stringReader = new StringReader(txt.text);
            string currentLine;

            while ((currentLine = stringReader.ReadLine()) != null)
            {

                if (currentLine.StartsWith("//"))
                    continue;

                if (currentLine.Length == 0)
                    continue;

                // Load 
                {
                    TextAsset iniText = (TextAsset)Resources.Load(currentLine);

                    if (iniText == null)
                    {
                        Debug.Log("ArchLoader: failed to locate listed file - " + currentLine);
                        continue;
                    }

                    INIReader reader = new INIReader(iniText.text);

                    T archObj = null;

                    foreach (INIReaderHeader h in reader.GetHeaders())
                    {
                        if ((h.nickname.CompareTo(masterSection) == 0) && (archObj == null))
                        {
                            archObj = new T();
                            archObj.ReadHeader(h);
                        }
                        else if ((h.nickname.CompareTo(masterSection) != 0) && (archObj != null))
                        {
                            archObj.ReadHeader(h);
                        }
                        else if ((h.nickname.CompareTo(masterSection) == 0) && (archObj != null))
                        {
                            // Store previous
                            outVal.Add(archObj);

                            // Create new and read current section
                            archObj = new T();
                            archObj.ReadHeader(h);
                        }
                    }

                    if (archObj != null)
                        outVal.Add(archObj);

                }
            }

            outVal.BuildIndexData();
        }

        public static void Load(Dictionary<ulong, T> outVal, string assetFile, string masterSection)
        {
            TextAsset txt = (TextAsset)Resources.Load(assetFile);

            if (txt == null)
            {
                Debug.Log("ArchLoader: failed to locate file - " + assetFile);
                return;
            }

            // now loop through all strings
            StringReader stringReader = new StringReader(txt.text);
            string currentLine;

            while ((currentLine = stringReader.ReadLine()) != null)
            {

                if (currentLine.StartsWith("//"))
                    continue;

                if (currentLine.Length == 0)
                    continue;

                // Load 
                {
                    TextAsset iniText = (TextAsset)Resources.Load(currentLine);

                    if (iniText == null)
                    {
                        Debug.Log("ArchLoader: failed to locate listed file - " + currentLine);
                        continue;
                    }

                    INIReader reader = new INIReader(iniText.text);

                    T archObj = null;

                    foreach (INIReaderHeader h in reader.GetHeaders())
                    {
                        if ((h.nickname.CompareTo(masterSection) == 0) && (archObj == null))
                        {
                            archObj = new T();
                            archObj.ReadHeader(h);
                        }
                        else if ((h.nickname.CompareTo(masterSection) != 0) && (archObj != null))
                        {
                            archObj.ReadHeader(h);
                        }
                        else if ((h.nickname.CompareTo(masterSection) == 0) && (archObj != null))
                        {
                            // Store previous
                            outVal[archObj.id] = archObj;

                            // Create new and read current section
                            archObj = new T();
                            archObj.ReadHeader(h);
                        }
                    }

                    if (archObj != null)
                    {
                        outVal[archObj.id] = archObj;
                    }

                }
            }
        }
    }
}
