using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;
using FreeUniverse.Core;
using FreeUniverse.Common;

namespace FreeUniverse.World
{
    public interface IWorldObjectComponentDelegate
    {
        void OnWorldObjectComponentRelease(WorldObjectComponent component);
    }

    public class WorldObjectComponent
    {
        public static int MAX_ATTACH_DEPTH = 2;

        private List<IWorldObjectComponentDelegate> delegates { get; set; }
        public ulong guid { get; set; }
        public int id { get; private set; }
        public ArchWorldObjectComponent arch { get; private set; }
        public WorldObject parentWorldObject { get; private set; }
        public WorldObjectComponent parentComponent { get; private set; }
        public WorldObjectComponentPropertyHull hull { get; private set; }
        public WorldObjectComponentPropertyEngine engine { get; private set; }
        public WorldObjectComponentHardpoint connectionHardpoint { get; set; }
        public WorldObjectComponentPropertyWeapon weapon { get; private set; }
        public ListEx<WorldObjectComponent> subComponents { get; private set; }
        public IndexArray<WorldObjectComponentProperty> indexProperties { get; private set; }
        public Dictionary<ulong, WorldObjectComponentHardpoint> hardpoints { get; private set; }
        public int layer { get { return parentWorldObject.worldController.layer; } }

        public const int COMPONENT_ROOT = 0;

        public void AddDelegate(IWorldObjectComponentDelegate del)
        {
            delegates.Add(del);
        }

        public void RemoveDelegate(IWorldObjectComponentDelegate del)
        {
            delegates.Remove(del);
        }

        public WorldObjectComponentHardpoint GetHardpoint(ulong id)
        {
            WorldObjectComponentHardpoint e = null;
            hardpoints.TryGetValue(id, out e);
            return e;
        }

        void PostInit()
        {
            foreach (WorldObjectComponentProperty e in indexProperties)
                e.PostInit();

            foreach (WorldObjectComponent e in subComponents)
                e.PostInit();
        }

        public bool AttachComponent(ArchWorldObjectComponent archData, ulong hp)
        {
            WorldObjectComponent comp = new WorldObjectComponent(archData, parentWorldObject);
            comp.parentComponent = this;
            comp.connectionHardpoint = GetHardpoint(hp);
            comp.PostInit();
            subComponents.Add(comp);
            return true;
        }

        public WorldObjectComponent(ArchWorldObjectComponent arch, WorldObject parent)
        {
            delegates = new List<IWorldObjectComponentDelegate>();
            hardpoints = new Dictionary<ulong, WorldObjectComponentHardpoint>();
            subComponents = new ListEx<WorldObjectComponent>();
            indexProperties = new IndexArray<WorldObjectComponentProperty>(32);

            this.arch = arch;
            parentWorldObject = parent;
            id = parentWorldObject.indexComponents.Insert(this);

            if (arch.hull != null) hull = new WorldObjectComponentPropertyHull(this, arch.hull);

            // NOTE: hardpoints are always attached to the hull!!!!
            AttachHardpoints(arch);
            if (arch.engine != null)
            {
                engine = new WorldObjectComponentPropertyEngine(this, arch.engine);
                engine.Activate(true);
            }

            if (arch.weapon != null)
            {
                weapon = new WorldObjectComponentPropertyWeapon(this, arch.weapon);
                weapon.Activate(true);
            }

            // attach subcomponents
            foreach (ArchSubComponent e in arch.subComponents)
            {
                ArchWorldObjectComponent archComp = ArchModel.GetComponent(e.arch);
                if (archComp == null) continue;
                AttachComponent(archComp, e.hp);
            }
        }

        public void Update(float time)
        {

            if (engine != null) engine.Update(time);
            if (weapon != null) weapon.Update(time);

            if (hull != null)
            {
                hull.Update(time);

                if (hull.hullStructure != null && !hull.hullStructure.invincible && hull.hullStructure.structurePoints <= 0.0f)
                {
                    if (parentComponent == null)
                        parentWorldObject.worldController.DestroyWorldObjectComponent(parentWorldObject.id, WorldObjectComponent.COMPONENT_ROOT);
                    else
                        parentWorldObject.worldController.DestroyWorldObjectComponent(parentWorldObject.id, parentComponent.id);
                }
            }

            foreach (WorldObjectComponent e in subComponents)
                e.Update(time);
        }

        public void FireAllActiveWeapons()
        {
            if (weapon != null)
                weapon.Fire();

            foreach (WorldObjectComponent e in subComponents)
                e.FireAllActiveWeapons();

        }

        void AttachHardpoints(ArchWorldObjectComponent arch)
        {
            foreach (ArchObjectHardpoint e in arch.hardpoints)
            {
                WorldObjectComponentHardpoint hp = new WorldObjectComponentHardpoint(e, this);
                hardpoints[hp.arch.id] = hp;
            }
        }

        public Vector3 hullVelocity
        {
            get
            {
                if (hull != null)
                    return hull.worldRigidBody.velocity;

                return new Vector3();
            }
        }

        public float InflictDamageToHull(ProjectileDesc desc)
        {
            hull.hullStructure.InflictDamage(DamageType.Energy, desc.projectile_damage_energy);
            hull.hullStructure.InflictDamage(DamageType.DarkMatter, desc.projectile_damage_darkmatter);
            hull.hullStructure.InflictDamage(DamageType.Explosive, desc.projectile_damage_explosion);
            hull.hullStructure.InflictDamage(DamageType.Projectile, desc.projectile_damage_projectile);
            return hull.hullStructure.structurePoints;
        }

        bool _released = false;

        public void Release()
        {
            if (_released) return;

            _released = true;

            foreach (IWorldObjectComponentDelegate d in delegates)
                d.OnWorldObjectComponentRelease(this);

            parentWorldObject.indexComponents.Remove(this);


            foreach (WorldObjectComponent e in subComponents)
                e.Release();

            foreach (WorldObjectComponentProperty e in indexProperties)
            {
                if (e != null) e.Release();
            }

            foreach (KeyValuePair<ulong, WorldObjectComponentHardpoint> e in hardpoints)
                e.Value.Release();

        }
    }
}
