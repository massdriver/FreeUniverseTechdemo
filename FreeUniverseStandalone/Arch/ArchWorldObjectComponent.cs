using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World;
using UnityEngine;
using FreeUniverse.Common;

namespace FreeUniverse.Arch
{
    public struct ArchSubComponent
    {
        public ulong arch;
        public ulong hp;
    }

    public class ArchWorldObjectComponent : ArchObject
    {
        ulong _universalPrice = 0; // price in universal credits
        public ulong universalPrice { get { return _universalPrice; } }

        protected ComponentType _type = ComponentType.Null;
        public ComponentType type { get { return _type; } }

        ArchObjectHardpoint _connectionHardpoint = new ArchObjectHardpoint();
        public ArchObjectHardpoint connectionHardpoint { get { return _connectionHardpoint; } }

        List<ArchObjectHardpoint> _hardpoints = new List<ArchObjectHardpoint>();
        public List<ArchObjectHardpoint> hardpoints { get { return _hardpoints; } }

        ArchWorldObjectComponentPropertyHull _hull;
        public ArchWorldObjectComponentPropertyHull hull { get { return _hull; } }

        ArchWorldObjectComponentPropertyEngine _engine;
        public ArchWorldObjectComponentPropertyEngine engine { get { return _engine; } }

        ArchWorldObjectComponentPropertyWeapon _weapon;
        public ArchWorldObjectComponentPropertyWeapon weapon { get { return _weapon; } }

        ulong _requiredConnectionHardpoint = HashUtils.StringToUINT64("hp_type_any");
        public ulong requiredConnectionHardpoint { get { return _requiredConnectionHardpoint; } }

        List<ArchSubComponent> _subComponents = new List<ArchSubComponent>();
        public List<ArchSubComponent> subComponents { get { return _subComponents; } }

        public override void ReadHeader(INIReaderHeader header)
        {
            if (header.Check("SubComponents"))
            {
                foreach (INIReaderParameter p in header.parameters)
                {
                    ArchSubComponent comp = new ArchSubComponent();
                    comp.arch = p.GetStrkey64(0);
                    comp.hp = p.GetStrkey64(1);
                    _subComponents.Add(comp);
                }
            }
            else if (header.Check("Component"))
            {
                base.ReadHeader(header);

                foreach (INIReaderParameter p in header.parameters)
                {
                    if (p.Check("required_hardpoint_type"))
                    {
                        _requiredConnectionHardpoint = p.GetStrkey64(0);
                    }
                }
            }
            else if (header.Check("Hull"))
            {
                ReadHull(header);
            }
            else if (header.Check("Hardpoints"))
            {
                ReadHardpoints(header);
            }
            else if (header.Check("Engine"))
            {
                ReadEngine(header);
            }
            else if (header.Check("Weapon"))
            {
                ReadWeapon(header);
            }
        }

        void ReadWeapon(INIReaderHeader header)
        {
            _weapon = new ArchWorldObjectComponentPropertyWeapon();
            _weapon.ReadHeader(header);
        }

        void ReadEngine(INIReaderHeader header)
        {
            _engine = new ArchWorldObjectComponentPropertyEngine();
            _engine.ReadHeader(header);
        }

        void ReadHull(INIReaderHeader header)
        {
            _hull = new ArchWorldObjectComponentPropertyHull();
            _hull.ReadHeader(header);
        }

        void ReadHardpoints(INIReaderHeader header)
        {
            foreach (INIReaderParameter p in header.parameters)
            {
                if (p.Check("hardpoint"))
                {
                    ArchObjectHardpoint hp = new ArchObjectHardpoint();
                    hp.ReadParameter(p);
                    _hardpoints.Add(hp);
                    continue;
                }
                else if (p.Check("connection_hardpoint"))
                {
                    _connectionHardpoint = new ArchObjectHardpoint();
                    _connectionHardpoint.ReadParameter(p);
                    continue;
                }
            }
        }
        
        public static ComponentType StringToWorldObjectComponentType(string str)
        {
            if (str.CompareTo("root") == 0)
                return ComponentType.Root;

            if (str.CompareTo("slave") == 0)
                return ComponentType.Slave;

            return ComponentType.Null;
        }
    }
}
