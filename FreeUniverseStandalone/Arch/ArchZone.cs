using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Arch
{
    public class ArchZoneExclusion
    {
        public ulong nickname;
        public ZoneShapeType shape;
        public Vector3 size;
        public Vector3 position;
    }

    public class ArchZoneElement
    {
        public string asset;
        public float chance; // 0.0 - 1.0
        public int minCount = 1, maxCount = 1;
        
        public Vector3 minScale;
        public Vector3 maxScale;

        public Vector3 minRotation;
        public Vector3 maxRotation;
        public bool fullRandomRotation = true;
        public bool uniformScale = true;
        public bool serverShouldGenerate = false;
        public bool createMeshCollider = false;

        public void Read(INIReaderHeader h)
        {
            foreach (INIReaderParameter p in h.parameters)
            {
                if (p.Check("create_mesh_collider"))
                {
                    createMeshCollider = (p.GetInt(0) == 1);
                    continue;
                }
                else
                if (p.Check("asset"))
                {
                    asset = p.GetString(0);
                    continue;
                }
                else if (p.Check("uniform_scale"))
                {
                    uniformScale = (p.GetInt(0) == 1);
                    continue;
                }
                else if (p.Check("chance"))
                {
                    chance = p.GetFloat(0);
                    continue;
                }
                else if (p.Check("min_scale"))
                {
                    minScale = p.GetVector3(0);
                    continue;
                }
                else if (p.Check("max_scale"))
                {
                    maxScale = p.GetVector3(0);
                    continue;
                }
                else if (p.Check("full_random_rotation"))
                {
                    fullRandomRotation = (p.GetInt(0) == 1);
                    continue;
                }
                else if (p.Check("min_rotation"))
                {
                    minRotation = p.GetVector3(0);
                    continue;
                }
                else if (p.Check("max_rotation"))
                {
                    maxRotation = p.GetVector3(0);
                    continue;
                }
                else if (p.Check("server_should_generate"))
                {
                    serverShouldGenerate = p.GetBool(0);
                    continue;
                }
                else if (p.Check("count_per_cube"))
                {
                    minCount = p.GetInt(0);
                    maxCount = p.GetInt(1);
                    continue;
                }
            }
        }
    }

    public class ArchZoneDust
    {
        public int maxCount;
        public float chance;
        public string asset;
        public float mass;
        public float randomVelocity;
        public Vector3 minScale;
        public Vector3 maxScale;
        public float visibilityRange = 250.0f;
        public float collisionRadius = 1.0f;
        public float refreshTime = 5.0f;

        public void Read(INIReaderHeader h)
        {
            foreach (INIReaderParameter p in h.parameters)
            {
                if (p.Check("asset"))
                {
                    asset = p.GetString(0);
                    continue;
                }
                else if (p.Check("refresh_time"))
                {
                    refreshTime = p.GetFloat(0);
                    continue;
                }
                else if (p.Check("max_count"))
                {
                    maxCount = p.GetInt(0);
                    continue;
                }
                else if (p.Check("chance"))
                {
                    chance = p.GetFloat(0);
                    continue;
                }
                else if (p.Check("mass"))
                {
                    mass = p.GetFloat(0);
                    continue;
                }
                else if (p.Check("random_velocity"))
                {
                    randomVelocity = p.GetFloat(0);
                    continue;
                }
                else if (p.Check("collision_radius"))
                {
                    collisionRadius = p.GetFloat(0);
                    continue;
                }
                else if (p.Check("visibility_range"))
                {
                    visibilityRange = p.GetFloat(0);
                    continue;
                }
                else if (p.Check("min_scale"))
                {
                    minScale = p.GetVector3(0);
                    continue;
                }
                else if (p.Check("max_scale"))
                {
                    maxScale = p.GetVector3(0);
                    continue;
                }
            }
        }
    }

    public class ArchZone : ArchObject
    {
        float _cubeSize = 1000.0f;
        public float cubeSize { get { return _cubeSize; } }

        List<ArchZoneElement> _innerElements = new List<ArchZoneElement>();
        public List<ArchZoneElement> innerElements { get { return _innerElements; } }

        List<ArchZoneElement> _outerElements = new List<ArchZoneElement>();
        public List<ArchZoneElement> outerElements { get { return _outerElements; } }

        List<ArchZoneDust> _dust = new List<ArchZoneDust>();
        public List<ArchZoneDust> dust { get { return _dust; } }

        //List<ArchZoneExclusion> _exclusions = new List<ArchZoneExclusion>();
        //public List<ArchZoneExclusion> exclusions { get { return _exclusions; } }

        string _background;
        public string background { get { return _background; } }

        public override void ReadHeader(INIReaderHeader header)
        {
            if (header.Check("OuterElement"))
            {
                ArchZoneElement e = new ArchZoneElement();
                e.Read(header);
                _outerElements.Add(e);
                return;
            }
            else if (header.Check("Dust"))
            {
                ArchZoneDust e = new ArchZoneDust();
                e.Read(header);
                _dust.Add(e);
                return;
            }
            else
            if (header.Check("InnerElement"))
            {
                ArchZoneElement e = new ArchZoneElement();
                e.Read(header);
                _innerElements.Add(e);
                return;
            }
            else
            if (header.Check("Zone"))
            {
                base.ReadHeader(header);

                foreach (INIReaderParameter p in header.parameters)
                {
                    if (p.Check("cube_size"))
                    {
                        _cubeSize = p.GetFloat(0);
                        continue;
                    }
                    else if (p.Check("background"))
                    {
                        _background = p.GetString(0);
                        continue;
                    }
                }
            }
        }
    }
}
