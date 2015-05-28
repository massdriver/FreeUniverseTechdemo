using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Arch
{
    public enum SolarType
    {
        Null,
        Static,
        Star,
        Planet,
        Base
    }

    public class ArchObjectSystemSolar : ArchObject
    {
        public ArchObjectBaseContents baseContentsArch { get; set; }
        public SolarType solarObjectType { get; set; }
        public string componentArchName { get; set; }
        public ulong componentArchID { get; set; }
        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public Vector3 scale { get; set; }
        public Vector3 spin { get; set; }
        //public uint solarType { get; set; }
        public float starLightDistance { get; set; }

        public static readonly uint SOLAR_TYPE_PLANET = HashUtils.StringToUINT32("planet");
        public static readonly uint SOLAR_TYPE_STAR = HashUtils.StringToUINT32("star");
        public static readonly uint SOLAR_TYPE_BASE = HashUtils.StringToUINT32("base");
        public static readonly uint SOLAR_TYPE_STATIC = HashUtils.StringToUINT32("static");

        public ArchObjectSystemSolar()
        {
            scale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        public static SolarType KeyToSolarType(uint key)
        {
            if (key == SOLAR_TYPE_BASE) return SolarType.Base;
            if (key == SOLAR_TYPE_STAR) return SolarType.Star;
            if (key == SOLAR_TYPE_PLANET) return SolarType.Planet;
            if (key == SOLAR_TYPE_STATIC) return SolarType.Static;

            return SolarType.Null;
        }

        public override void ReadParameter(INIReaderParameter parameter)
        {
            base.ReadParameter(parameter);

            if (parameter.Check("position"))
            {
                position = parameter.GetVector3(0);
            }
            else if (parameter.Check("base_arch"))
            {
                baseContentsArch = ArchModel.GetBase(parameter.GetStrkey64(0));
            }
            else if (parameter.Check("rotation"))
            {
                rotation = parameter.GetVector3(0);
            }
            else if (parameter.Check("scale"))
            {
                scale = parameter.GetVector3(0);
            }
            else if (parameter.Check("spin"))
            {
                spin = parameter.GetVector3(0);
            }
            else if (parameter.Check("solar_type"))
            {
                solarObjectType = KeyToSolarType(parameter.GetStrkey(0));
            }
            else if (parameter.Check("star_light_distance"))
            {
                starLightDistance = parameter.GetFloat(0);
            }
            else if (parameter.Check("component_arch"))
            {
                componentArchID = parameter.GetStrkey64(0);
                componentArchName = parameter.GetString(0);
            }

            
        }
    }
}
