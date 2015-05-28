using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.World
{
    public struct WorldObjectStats
    {
        public float speed, maxSpeed;
        public float energy, maxEnergy;
        public float shield, maxShield;
        public float hull, maxHull;
        public float radarRange;
    }

    public class WorldObjectControlPanel : IWorldObjectDelegate
    {
        WorldObject _worldObject;
        public WorldObject worldObject { get { return _worldObject; } }

        public WorldObjectControlPanel(WorldObject obj)
        {
            _worldObject = obj;
        }

        public WorldObjectStats GetWorldObjectBaseStats()
        {
            WorldObjectStats stats = new WorldObjectStats();
            stats.maxSpeed = 500.0f;
            stats.radarRange = 20000.0f;

            if (_worldObject == null)
            {
                
                stats.maxEnergy = 1.0f;
                stats.maxHull = 1.0f;
                stats.maxShield = 1.0f;
                return stats;
            }

            int avgCount = 0;
            foreach (WorldObjectComponent c in _worldObject.components)
            {
                if (c.hull != null)
                {
                    stats.speed += c.hull.velocity.magnitude;
                    avgCount++;

                    stats.maxHull += c.hull.arch.structurePoints;
                    stats.hull += c.hull.hullStructure.structurePoints;
                }
            }

            if (avgCount != 0) stats.speed /= avgCount;

            return stats;
        }

        public void Update(float time)
        {

        }

        public uint GetAllActiveWeaponsMask()
        {
            if (_worldObject == null) return 0;

            uint weaponMask = 0;
            foreach (WorldObjectComponentPropertyWeapon w in _worldObject.indexWeapons)
            {
                if (w.Fire())
                    weaponMask += (uint)(1 << (int)(w.weaponID));
            }

            return weaponMask;
        }

        float _targetPointCrossDistance = 2000.0f;
        public float targetPointCrossDistance { get { return _targetPointCrossDistance; } set { _targetPointCrossDistance = value; } }

        // used by weapons
        Vector3 _weaponAimPoint;
        public Vector3 weaponAimPoint { get { return _weaponAimPoint; } set { _weaponAimPoint = value; } } 

        bool _movementControlsActive = false;
        public bool movementControlsActive { get { return _movementControlsActive; } set { _movementControlsActive = value; } }

        public void SetEngineControlValues(
            float thrustValueForwardBackward, float thrustValueUpDown, float thrustValueLeftRight,
            float torqueValueYaw, float torqueValuePitch, float torqueValueRoll )
        {
            _thrustValueForwardBackward = thrustValueForwardBackward;

            if (_worldObject == null) return;

            foreach (WorldObjectComponent c in _worldObject.components)
            {
                if(c.engine!=null)
                    c.engine.SetControlValues(
                        thrustValueForwardBackward, thrustValueUpDown, thrustValueLeftRight,
                        torqueValueYaw, torqueValuePitch, torqueValueRoll );
            }
        }

        float _thrustValueForwardBackward = 0.0f;
        public float thrustValueForwardBackward { get { return _thrustValueForwardBackward; } }

        #region IWorldObjectDelegate Members

        public void OnWorldObjectReleased(WorldObject obj)
        {
            _worldObject = null;
        }

        #endregion
    }
}
