using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;
using FreeUniverse.World.Zone;
using FreeUniverse.Core;
using FreeUniverse.Common;

namespace FreeUniverse.World
{
    public enum SimulationType
    {
        Client,
        Server
    }

    // NOTE: Root component should always have hull or otherwise some parts may crash
    public class WorldObject : IZoneObserver, IObjectInspector, IReleasable
    {
        private static ulong GUID_VALUE = 128;
        public static ulong MakeGUID() { GUID_VALUE++; return GUID_VALUE; }

        public WorldObjectType type { get; set; }
        public WorldObjectComponent rootComponent { get; private set; }
        public ulong guid { get; set; } // guid is used for projectile tests
        public int id { get; private set; }
        public WorldController worldController { get; private set; }
        public SimulationType simulationType { get; set; }
        public string objectName { get; set; }
        public List<WorldObjectComponent> components { get; private set; }
        public WorldObjectControlPanel controlPanel { get; private set; }
        public IndexArray<WorldObjectComponent> indexComponents { get; private set; }
        public IndexArray<WorldObjectComponentPropertyWeapon> indexWeapons { get; private set; }
        public WorldObjectController controller { get; private set; }

        public ArchObjectSystemSolar solarObjectData { get; set; }
        public BaseContents baseContents { get; set; }

        public const int INVALID_WORLD_OBJECT_ID = -1;

        private List<IWorldObjectDelegate> delegates { get; set; }

        public bool isDockable
        {
            get
            {
                return baseContents != null;
            }
        }

        public string hudTargetObjectName
        {
            get
            {
                if (solarObjectData != null)
                    return solarObjectData.strName;

                return objectName;
            }
        }

        public DockReply RequestDock(WorldObject obj)
        {
            return DockReply.Ok;
        }

        public float averageWeaponSpeed
        {
            get
            {
                float spd = 0.0f;
                int i = 0;

                foreach (WorldObjectComponentPropertyWeapon w in indexWeapons)
                {
                    spd += w.arch.projectileDesc.projectile_launch_speed;
                    i++;
                }

                if (i != 0) return spd / (float)i;

                return 0.0f;
            }
        }

        public void SetController(WorldObjectController newController )
        {
            controller = newController;
            controller.SetWorldObject(this);
        }

        // for server 
        public WorldObject(WorldController controller, int objectid, ArchWorldObjectComponent root, ulong guid, Vector3 pos) :
            this(controller, objectid, root)
        {
            this.guid = guid;
            SetRootHullPosition(pos);
        }

        public const int MAX_WEAPONS = 16;

        // for client offline
        public WorldObject(WorldController controller, int objectid, ArchWorldObjectComponent root)
        {
            delegates = new List<IWorldObjectDelegate>();
            components = new List<WorldObjectComponent>();
            indexWeapons = new IndexArray<WorldObjectComponentPropertyWeapon>(MAX_WEAPONS);
            indexComponents = new IndexArray<WorldObjectComponent>(64);
            objectName = "Unknown";
            released = false;
            simulationType = SimulationType.Server;
            type = WorldObjectType.Null;

            worldController = controller;
            id = objectid;
            guid = MakeGUID();
            controlPanel = new WorldObjectControlPanel(this);

            AddDelegate(controlPanel);
            CreateRoot(root);
        }

        public WorldObjectComponentHardpoint GetRootHullHardpoint(string name)
        {
            if (rootComponent == null)
                return null;

            return rootComponent.hardpoints[HashUtils.StringToUINT64(name)];
        }

        public WorldRigidBody GetRootHullRigidBody()
        {
            if (rootComponent == null)
                return null;

            return rootComponent.hull.worldRigidBody;
        }

        public void SetRootHullPosition( Vector3 pos )
        {
            if (rootComponent == null)
                return;

            rootComponent.hull.SetPosition(pos);
        }

        public Vector3 rootHullPosition
        {
            get
            {
                if (rootComponent == null || rootComponent.hull == null || released)
                    return new Vector3();

                return rootComponent.hull.hullGlobalPosition;
            }
        }

        public WorldObjectComponent root
        {
            get { return rootComponent; }
        }

        public bool CreateRoot(ArchWorldObjectComponent root)
        {
            if (root == null)
                return false;

            WorldObjectComponent component = new WorldObjectComponent(root, this);
            //component.parentComponent = null;
            components.Add(component);
            rootComponent = component;
            return true;
        }

        public int AddComponent(WorldObjectComponent component)
        {
            components.Add(component);
            return components.Count - 1;
        }

        public WorldObjectComponent GetComponent(int id)
        {
            if (id >= components.Count)
                return null;

            return components[id];
        }

        public void AddDelegate(IWorldObjectDelegate del)
        {
            delegates.Add(del);
        }

        public void RemoveDelegate(IWorldObjectDelegate del)
        {
            delegates.Remove(del);
        }

        public void Update(float time)
        {
            if (!IsAlive()) return;

            if (controller != null)
                controller.Update(time);

            foreach (WorldObjectComponent e in indexComponents)
            {
                if( e != null ) e.Update(time);
            }
        }

        public bool DrawEnergy(float amount)
        {
            if (amount <= 0.0f)
                return true;

            return true;
        }

        public bool IsAlive()
        {
            foreach (WorldObjectComponent e in indexComponents)
            {
                if (e != null && e.hull != null && !e.hull.dead)
                    return true;

            }

            return false;
        }

        public bool released { get; private set; }

        public void Release()
        {
            if (released) return;

            released = true;

            foreach (WorldObjectComponent e in components)
                e.Release();

            components.Clear();
            components = null;

            foreach (IWorldObjectDelegate d in delegates)
                d.OnWorldObjectReleased(this);

            delegates.Clear();
            delegates = null;
        }

        #region IZoneGeneratorObserver Members

        public bool zoneObserverShouldCreateDust
        {
            get { return true; }
        }

        public Vector3 zoneObserverPosition
        {
            get { return rootHullPosition; }
        }

        public float zoneObserverVisibilityRange
        {
            get { return 5000.0f; }
        }

        #endregion

        #region IRemovableFromCollection Members

        public bool CanBeReleased()
        {
            return !IsAlive();
        }

        #endregion
    }
}
