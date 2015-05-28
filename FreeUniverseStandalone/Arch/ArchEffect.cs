using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Arch
{
    public enum EffectComponentType
    {
        Prefab
    }

    public struct ArchEffectComponent
    {
        public EffectComponentType type;
        public string prefab;
        public float predelay;
        public float lifetime;

        EffectComponentType EffectComponentTypeFromString(string s)
        {
            return EffectComponentType.Prefab;
        }

        public void Read(INIReaderHeader header)
        {
            foreach (INIReaderParameter p in header.parameters)
            {
                if (p.Check("prefab"))
                {
                    prefab = p.GetString(0);
                }
                else if (p.Check("type"))
                {
                    type = EffectComponentTypeFromString(p.GetString(0));
                }
                else if (p.Check("predelay"))
                {
                    predelay = p.GetFloat(0);
                }
                else if (p.Check("lifetime"))
                {
                    lifetime = p.GetFloat(0);
                }
            }
        }
    }

    public class ArchEffect : ArchObject
    {
        List<ArchEffectComponent> _components = new List<ArchEffectComponent>();
        public List<ArchEffectComponent> components { get { return _components; } }

        public override void ReadHeader(INIReaderHeader header)
        {
            if (header.Check("Component"))
            {
                ArchEffectComponent e = new ArchEffectComponent();
                e.Read(header);
                _components.Add(e);
                return;
            }
            else
                base.ReadHeader(header);
        }
    }
}
