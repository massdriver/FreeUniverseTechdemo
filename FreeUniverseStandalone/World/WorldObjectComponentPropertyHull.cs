using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;
using FreeUniverse.Common;

namespace FreeUniverse.World
{
    public class WorldObjectComponentPropertyHull : WorldObjectComponentProperty
    {
        ArchWorldObjectComponentPropertyHull _arch; 
        public ArchWorldObjectComponentPropertyHull arch { get { return _arch; } }

        WorldRigidBody _worldRigidBody;
        public WorldRigidBody worldRigidBody { get { return _worldRigidBody; } }

        HullStructureData _hullStructure;
        public HullStructureData hullStructure { get { return _hullStructure; } }

        public bool dead { get; set; }

        public WorldObjectComponentPropertyHull(WorldObjectComponent parent, ArchWorldObjectComponentPropertyHull arch)
            : base(parent, arch)
        {
            _arch = arch;
            _hullStructure = new HullStructureData(arch);
            _worldRigidBody = new WorldRigidBody(arch.model, arch.mass, true, parent.layer, this);
            type = ComponentPropertyType.Hull;
            dead = false;
        }

        bool _released = false;

        public override void Release()
        {
            if (_released) return;

            _released = true;

            _worldRigidBody.Release();

            base.Release();

            dead = true;
        }

        public Vector3 velocity { get { if (_worldRigidBody != null) return _worldRigidBody.velocity; return new Vector3(); } }

        public void SetPosition(Vector3 pos)
        {
            if (_worldRigidBody == null)
                return;

            _worldRigidBody.gameObject.transform.position = pos;
        }

        public Vector3 hullGlobalPosition
        {
            get
            {
                if (_worldRigidBody == null)
                    return new Vector3();

                return _worldRigidBody.gameObject.transform.position;
            }
        }

        public Quaternion GetHullGlobalRotation()
        {
            if (_worldRigidBody == null)
                return new Quaternion();

            return _worldRigidBody.gameObject.transform.rotation;
        }

        public override void Update(float time)
        {

        }

        public void PlayDeathAnimation()
        {
            if (_released) return;

            if (_worldRigidBody == null)
                return;

            if (_arch.deathEffects.Count != 0)
            {
                ArchWorldObjectComponentPropertyHull.DeathEffect e = _arch.deathEffects[0];
                parentComponent.parentWorldObject.worldController.CreateEffect(e.prefab, hullGlobalPosition, e.time);
            }

            _worldRigidBody.Release();

            dead = true;
        }

    }
}
