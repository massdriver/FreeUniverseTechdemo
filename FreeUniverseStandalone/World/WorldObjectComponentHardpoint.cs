using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;

namespace FreeUniverse.World
{
    public class WorldObjectComponentHardpoint : BaseGameObject
    {
        WorldObjectComponent _parentComponent;
        public WorldObjectComponent parentComponent { get { return _parentComponent; } }

        ArchObjectHardpoint _arch;
        public ArchObjectHardpoint arch { get { return _arch; } }

        public Transform transform { get { return gameObject.transform; } }
        public Quaternion rotation { get { return gameObject.transform.rotation; } }
        public Vector3 position { get { return gameObject.transform.position; } }

        public WorldObjectComponentHardpoint(ArchObjectHardpoint arch, WorldObjectComponent parent)
        {
            _arch = arch;
            _parentComponent = parent;
            gameObject.transform.parent = parent.hull.worldRigidBody.gameObject.transform;
            gameObject.name = "hp_"+arch.nickname;

            gameObject.transform.Rotate(arch.rotation, Space.Self);
            gameObject.transform.Translate(arch.position, Space.Self);
        }

        public void Release()
        {
            if (_object == null) return;

            UnityEngine.Object.Destroy(_object);
            _object = null;
        }
    }
}
