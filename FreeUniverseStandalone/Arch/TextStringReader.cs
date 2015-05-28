using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace FreeUniverse.Arch
{
    public class TextStringReader
    {
        public static int LoadStrings(string[] outStrings, string str)
        {
            StringReader reader = new StringReader(str);

            int count = 0;
            int lineCount = 0;

            while (true)
            {
                string line = reader.ReadLine();

                if (line == null) break;

                if (line.StartsWith("//") || line.StartsWith("#")) continue;

                int delimiter = line.IndexOf('=');

                if (delimiter == -1) continue;

                string idstr = null;
                uint id = 0;

                try
                {
                    idstr = line.Substring(0, delimiter).Trim();
                    id = UInt32.Parse(idstr);
                }
                catch (Exception e)
                {
                    Debug.Log("Error while parsing string contents at line=" + lineCount);
                }

                string contents = line.Substring(delimiter + 1).Trim(CONTENT_TRIM_CHARS);
                outStrings[id] = contents;
                count++;
                lineCount++;
            }

            return count;
        }

        private static char[] CONTENT_TRIM_CHARS = { '"', ' ', '\t'};
    }
}
