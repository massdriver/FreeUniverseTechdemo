using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Arch;

namespace FreeUniverse.World
{
    public class WeaponProjectile : ProjectileDelegate
    {
        private GameObject gameObject { get; set; }
        private ProjectileBehaviour pb { get; set; }
        private WorldController controller { get; set; }
        public int componentArchIndex { get; private set; }
        public ulong componentArch { get; private set; }
        public ulong parentGUID { get; set; }
        private Vector3 velocity { get; set; }
        public ProjectileDesc projectileDesc { get; set; }

        public WeaponProjectile(WorldObjectComponentPropertyWeapon weapon, Vector3 targetPoint)
        {
            ProjectileLaunchInfo launchInfo = weapon.GetLaunchInfo();
            projectileDesc = weapon.arch.projectileDesc;

            componentArch = weapon.parentComponent.arch.id;
            componentArchIndex = weapon.parentComponent.arch.index;

            gameObject = Helpers.LoadUnityObject(weapon.arch.projectileDesc.projectile_prefab);
            gameObject.name = "proj";
            controller = weapon.parentComponent.parentWorldObject.worldController;

            Helpers.SetLayerDeep(gameObject, weapon.parentComponent.parentWorldObject.worldController.layer);

            parentGUID = weapon.parentComponent.parentWorldObject.guid;
            pb = gameObject.AddComponent<ProjectileBehaviour>();
            pb.projectileDelegate = this;
            lifetime = projectileDesc.projectile_lifetime;

            Vector3 axis = (targetPoint - launchInfo.launchPosition).normalized;
            gameObject.transform.position = launchInfo.launchPosition;
            gameObject.transform.rotation = Quaternion.LookRotation(axis);
            velocity = projectileDesc.projectile_launch_speed * axis + weapon.parentComponent.hullVelocity;

            // Note: _object should have rigidbody component and sphere or box collider
            Rigidbody rigid = gameObject.AddComponent<Rigidbody>();
            rigid.useGravity = false;
            rigid.mass = 1.0f;
            rigid.velocity = velocity;
        }

        bool _removeFlag = false;
        public bool removeFlag { get { return _removeFlag; } set { _removeFlag = value; gameObject.SetActive(!value); } }

        private float lifetime { get; set; }

        public void Update(float time)
        {
            if (_removeFlag) return;

            lifetime -= time;

            if (lifetime <= 0.0f)
            {
                _removeFlag = true;
                gameObject.SetActive(false);
            }

            //_object.transform.position = _object.transform.position + _velocity * time;
        }

        public void Release()
        {
            _removeFlag = true;
            pb = null;
            controller = null;
            projectileDesc = null;

            if (gameObject != null)
            {
                UnityEngine.Object.Destroy(gameObject);
                gameObject = null;
            }
        }

        #region ProjectileDelegate Members

        enum ProjectileTriggerType
        {
            Unknown,
            Hostile,
            Friendly,
            Projectile
        }

        ProjectileTriggerType CheckGUID(Collider other, out WorldObjectComponent target)
        {
            target = null;

            if (other.gameObject == null)
            {
                Release();
            }

            if (other.gameObject.name.CompareTo("proj") == 0) return ProjectileTriggerType.Projectile;

            RayCastProvider p = other.gameObject.GetComponent<RayCastProvider>();

            if (p == null) return ProjectileTriggerType.Hostile; // this doesnt support out logic

            target = p.worldRidigBody.parentHullProperty.parentComponent;

            // same world object
            if (p.worldRidigBody.parentHullProperty.parentComponent.parentWorldObject.guid == parentGUID)
                return ProjectileTriggerType.Friendly;

            return ProjectileTriggerType.Hostile;
        }

        public RaycastHit rayCastInfo { get; set; }

        public void OnTriggerEnter(Collider other)
        {
            if (gameObject == null)
                return;

            WorldObjectComponent comp;
            ProjectileTriggerType triggerType = CheckGUID(other, out comp);

            if (triggerType == ProjectileTriggerType.Friendly)
            {
                return;
            }

            // can be another projectile collider
            if (triggerType == ProjectileTriggerType.Projectile)
            {
                return;
            }
            
            Ray ray = new Ray(gameObject.transform.position, velocity.normalized);
            RaycastHit rayhit;

            if (gameObject.collider.Raycast(ray, out rayhit, 1000.0f))
            {
                rayCastInfo = rayhit;
                controller.HandleProjectileHit(this, comp);
            }
        }

        #endregion

        #region ProjectileDelegate Members


        public void OnCollisionEnter(Collision collision)
        {
            
        }

        #endregion
    }
}
