using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Arch
{
    public enum ZoneShapeType
    {
        Sphere
    }

    public class ArchObjectSystemZone : ArchObject
    {
        ZoneShapeType _shapeType;
        public ZoneShapeType shapeType { get { return _shapeType; } }

        Vector3 _position;
        public Vector3 position { get { return _position; } }

        Vector3 _size;
        public Vector3 size { get { return _size; } }

        uint _seed;
        public uint seed { get { return _seed; } }

        ulong _zoneArch;
        public ulong zoneArch { get { return _zoneArch; } }

        ZoneShapeType FromString(string str)
        {
            return ZoneShapeType.Sphere;
        }

        public override void ReadHeader(INIReaderHeader header)
        {
            base.ReadHeader(header);

            foreach (INIReaderParameter p in header.parameters)
            {
                if (p.Check("position"))
                {
                    _position = p.GetVector3(0);
                    continue;
                }
                else if (p.Check("size"))
                {
                    _size = p.GetVector3(0);
                    continue;
                }
                else if (p.Check("seed"))
                {
                    _seed = (uint)p.GetInt(0);
                    continue;
                }
                else if (p.Check("shape"))
                {
                    _shapeType = FromString(p.GetString(0));
                    continue;
                }
                else if (p.Check("zone_arch"))
                {
                    _zoneArch = p.GetStrkey64(0);
                    continue;
                }
            }
        }
    }
}
