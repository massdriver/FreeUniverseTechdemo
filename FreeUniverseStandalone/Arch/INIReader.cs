using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FreeUniverse;

namespace FreeUniverse.Arch
{
    public class INIReaderParameter
    {
        public string nickname;
        public string[] values;

        public INIReaderParameter(string[] values)
        {
            this.nickname = values[0];
            this.values = values;
        }

        public string GetName()
        {
            return values[0];
        }

        public bool Check(string name)
        {
            return nickname.CompareTo(name) == 0;
        }

        public string GetString(int id)
        {
            if (values.Length <= (id + 1))
                return null;

            return values[id + 1];
        }

        public uint GetStrkey(int id)
        {
            if (values.Length <= (id + 1))
                return 0;

            string s = values[id + 1];

            if (s.CompareTo("null") == 0)
                return 0;

            return HashUtils.StringToUINT32(s);
        }

        public ulong GetStrkey64(int id)
        {
            if (values.Length <= (id + 1))
                return 0;

            string s = values[id + 1];

            if (s.CompareTo("null") == 0)
                return 0;

            return HashUtils.StringToUINT64(s);
        }

        public int GetInt(int id)
        {
            if (values.Length <= (id + 1))
                return 0;

            return System.Int32.Parse(values[id + 1]);
        }

        public ulong GetUInt64(int id)
        {
            if (values.Length <= (id + 1))
                return 0;

            return System.UInt64.Parse(values[id + 1]);
        }

        public bool GetBool(int id)
        {
            try
            {
                if (GetInt(id) == 1) return true;
            }
            catch (System.FormatException e)
            {
                if (GetString(id).CompareTo("true") == 0) return true;
            }

            return false;
        }

        public float GetFloat(int id)
        {
            if (values.Length <= (id + 1))
                return 0.0f;

            return (float)System.Double.Parse(values[id + 1]);
        }

        public Vector2 GetVector2(int id)
        {
            if (values.Length <= (id + 1 + 1))
                return new Vector2(0.0f, 0.0f);

            return new Vector2(GetFloat(id), GetFloat(id + 1));
        }

        public Vector3 GetVector3(int id)
        {
            if (values.Length <= (id + 1 + 2))
                return new Vector3(0.0f, 0.0f, 0.0f);

            return new Vector3(GetFloat(id), GetFloat(id + 1), GetFloat(id + 2));
        }

        public Vector4 GetVector4(int id)
        {
            if (values.Length <= (id + 1 + 3))
                return new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

            return new Vector4(GetFloat(id), GetFloat(id + 1), GetFloat(id + 2), GetFloat(id + 3));
        }
    }

    public class INIReaderHeader
    {
        public string nickname;
        public List<INIReaderParameter> parameters = new List<INIReaderParameter>();

        public INIReaderHeader(string n)
        {
            this.nickname = n;
        }

        public bool Check(string name)
        {
            return nickname.CompareTo(name) == 0;
        }
    }

    public class INIReader
    {
        StringReader _reader;

        public INIReader(string contents)
        {
            ParseContents(contents);
        }

        List<INIReaderHeader> _headers = new List<INIReaderHeader>();

        public List<INIReaderHeader> GetHeaders()
        {
            return _headers;
        }

        private void ParseContents(string contents)
        {
            _reader = new StringReader(contents);
            INIReaderHeader currentHeader = null;

            while (0 == 0)
            {
                string line = _reader.ReadLine();

                if (line == null)
                {
                    if (currentHeader != null)
                        _headers.Add(currentHeader);

                    return;
                }
                else if (line.Length == 0)
                    continue;
                else if (line.StartsWith("\n"))
                    continue;
                else if (line.StartsWith("//"))
                    continue;
                else if (line.StartsWith("["))
                {
                    if (currentHeader != null)
                        _headers.Add(currentHeader);

                    currentHeader = ParseHeader(line);
                }
                else if (System.Char.IsLetter(line[0]))
                {
                    if (currentHeader != null)
                    {
                        INIReaderParameter p = ParseParameter(line);

                        if (p != null)
                            currentHeader.parameters.Add(p);
                    }
                }
            }
        }

        private static char[] paramSplitChars = { ',', '=', ' ', '	' };

        private INIReaderParameter ParseParameter(string line)
        {
            string[] values = line.Split(paramSplitChars, System.StringSplitOptions.RemoveEmptyEntries);

            if (values.Length == 0)
                return null;

            return new INIReaderParameter(values);
        }

        private static char[] headerTrimChars = { ' ', '	', '[', ']', '\n' };

        private INIReaderHeader ParseHeader(string line)
        {
            string headerName = line.Trim(headerTrimChars);

            if (headerName == null)
                return null;

            return new INIReaderHeader(headerName);
        }
    }


}
