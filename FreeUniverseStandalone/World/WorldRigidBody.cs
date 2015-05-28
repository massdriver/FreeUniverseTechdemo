using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;

namespace FreeUniverse.World
{
    public class WorldRigidBody : BaseGameObject
    {
        public Rigidbody rigidBody { get; private set; }
        public WorldObjectComponentPropertyHull parentHullProperty { get; private set; }

        public Vector3 velocity
        {
            get
            {
                if( rigidBody != null )
                    return rigidBody.velocity;

                return new Vector3();
            }
        }

        public WorldRigidBody(string model, float mass, bool convex, int physicsLayer, WorldObjectComponentPropertyHull parentHull) : base(true)
        {
            _object = Helpers.LoadUnityObject(model);
            _object.name = "hull_" + parentHull.parentComponent.parentWorldObject.id;
            _object.layer = parentHull.parentComponent.layer;
            _object.AddComponent<RayCastProvider>().worldRidigBody = this;
            parentHullProperty = parentHull;

            //_object.AddComponent<GameObjectAdapter>().objectDelegate = this;

            // sync all child layers
            {
                foreach (Transform child in _object.transform.root)
                {
                    child.gameObject.layer = _object.layer;
                }
            }

            // A dynamic rigid body
            if (mass > 0.0f)
            {
                rigidBody = gameObject.AddComponent<Rigidbody>();
                rigidBody.useGravity = false;
                rigidBody.useConeFriction = false;
                rigidBody.drag = 1.0f;
                rigidBody.angularDrag = 1.0f;
                rigidBody.mass = mass;

                //_object.transform.parent = parentHull.parent.gameObject.transform;

                int collidersCreated = 0;
                foreach (Transform child in _object.transform.root)
                {
                    if (!child.name.StartsWith("hitbox_"))
                        continue;

                    MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = convex;
                    child.gameObject.AddComponent<RayCastProvider>().worldRidigBody = this;
                    collidersCreated++;
                }

                // Create default shape
                if (collidersCreated == 0)
                {
                    MeshCollider meshCollider = _object.AddComponent<MeshCollider>();
                    meshCollider.convex = true;
                }
            }
            // A static mesh collider
            else
            {
                foreach (HullHitboxParameters p in parentHull.arch.hullHitboxes)
                {
                    switch (p.type)
                    {
                        case HullHitboxType.Sphere:
                            {
                                SphereCollider collider = _object.AddComponent<SphereCollider>();
                                collider.radius = p.a;
                            };
                            break;
                        case HullHitboxType.StaticMesh:
                            {
                                MeshCollider collider = _object.AddComponent<MeshCollider>();
                                collider.convex = false;
                            }
                            break;
                    }
                }

            }
        }

        private bool released = false;

        public void Release()
        {
            if (released) return;

            released = true;

            _object.GetComponent<RayCastProvider>().worldRidigBody = null;
            UnityEngine.Component.Destroy(_object.GetComponent<RayCastProvider>());

            if (_object == null)
                return;

            UnityEngine.Object.Destroy(_object);// dont remove instantly
            _object = null;
        }
    }
}
