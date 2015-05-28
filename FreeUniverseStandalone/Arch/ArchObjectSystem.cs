using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Arch
{
    [Serializable]
    public class ArchObjectSystem : ArchObject
    {
        string _musicDefault;
        string _musicTension;
        string _musicBattle;
        string _stars;
        string _background;
        Vector4 _backgroundColor;
        Dictionary<ulong, ArchObjectSystemSolar> _solars = new Dictionary<ulong, ArchObjectSystemSolar>();
        Dictionary<ulong, ArchObjectSystemZone> _zones = new Dictionary<ulong, ArchObjectSystemZone>();

        public string musicDefault { get { return _musicDefault; } }
        public string musicTension { get { return _musicTension; } }
        public string musicBattle { get { return _musicBattle; } }
        public Vector4 backgroundColor { get { return _backgroundColor; } }
        public string background { get { return _background; } }
        public string stars { get { return _stars; } }
        public Dictionary<ulong, ArchObjectSystemSolar> solars { get { return _solars; } }
        public Dictionary<ulong, ArchObjectSystemZone> zones { get { return _zones; } }

        override public void ReadHeader(INIReaderHeader header)
        {
            if (header.Check("System"))
            {
                foreach (INIReaderParameter p in header.parameters)
                    ReadParameter(p);
            }
            else if (header.Check("Zone"))
                ReadZone(header);
            else if (header.Check("Solar"))
                ReadSolar(header);
        }

        void ReadSolar(INIReaderHeader header)
        {
            ArchObjectSystemSolar solar = new ArchObjectSystemSolar();
            solar.ReadHeader(header);
            _solars[solar.id] = solar;
        }

        void ReadZone(INIReaderHeader header)
        {
            ArchObjectSystemZone zone = new ArchObjectSystemZone();
            zone.ReadHeader(header);
            _zones[zone.id] = zone;
        }

        override public void ReadParameter(INIReaderParameter parameter)
        {
            base.ReadParameter(parameter);

            if (parameter.Check("music_default"))
            {
                _musicDefault = parameter.GetString(0);
            }
            else if (parameter.Check("music_tension"))
            {
                _musicTension = parameter.GetString(0);
            }
            else if (parameter.Check("music_battle"))
            {
                _musicBattle = parameter.GetString(0);
            }
            else if (parameter.Check("background"))
            {
                _background = parameter.GetString(0);
            }
            else if (parameter.Check("stars"))
            {
                _stars = parameter.GetString(0);
            }
            else if (parameter.Check("backgroundColor"))
            {
                _backgroundColor = parameter.GetVector4(0);
            }
        }
    }
}
