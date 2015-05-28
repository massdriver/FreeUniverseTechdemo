using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using FreeUniverse.Net;
using UnityEngine;
using FreeUniverse.World.Zone;
using FreeUniverse.Core;
using System.Collections;
using FreeUniverse.Net.Messages;

namespace FreeUniverse.World
{
    public enum DockReply
    {
        Error,
        Ok,
        DeniedTooFar,
        DeniedHostile,
        DeniedAccess,
        NoDock
    }

    public class WorldController : NetworkMessageHandler
    {
        public List<IZoneObserver> zoneObservers { get; private set; }
        public int layer { get; private set; }
        public bool paused { get; set; }
        public ulong system { get; set; }
        public ArchObjectSystem systemArch { get; private set; }

        protected IndexArray<WorldObject> worldObjects { get; set; }
        protected Dictionary<ulong, ZoneGenerator> zoneGenerators { get; set; }
        protected ListEx<WeaponProjectile> projectiles { get; set; }

        protected IZoneObserver _visualObserver;
        public IZoneObserver visualObserver { get { return _visualObserver; } set { _visualObserver = value; OnVisualObserverChanged(); } }

        public const int MAX_WORLD_OBJECTS = 4096;

        public WorldController(int layer)
        {
            projectiles = new ListEx<WeaponProjectile>();
            worldObjects = new IndexArray<WorldObject>(MAX_WORLD_OBJECTS);
            zoneGenerators = new Dictionary<ulong, ZoneGenerator>();
            zoneObservers = new List<IZoneObserver>();

            InitNetMessageHandlers();

            this.layer = layer;
        }
        
        protected virtual void OnVisualObserverChanged()
        {
            foreach (KeyValuePair<ulong, ZoneGenerator> e in zoneGenerators)
                e.Value.OnVisualObserverChanged();
        }

        public virtual void DestroyWorldObjectComponent(int obj, int component)
        {
            WorldObject e = TryGetValidWorldObject(obj);

            if (e == null) return;

            e.indexComponents[component].hull.PlayDeathAnimation();

            if (component == WorldObjectComponent.COMPONENT_ROOT)
                RemoveZoneObserver(e);
        }

        public virtual void Update(float time)
        {
            // Remove here or no death anim will be played
            foreach (WorldObject e in worldObjects)
            {
                e.Update(time);

                if (e.CanBeReleased())
                {
                    RemoveWorldObject(e.id);
                }
            }
            
            foreach (KeyValuePair<ulong, ZoneGenerator> e in zoneGenerators)
                e.Value.Update(time);

            // Update projectiles
            {
                WeaponProjectile p = projectiles.Iterate();
                while (p != null)
                {
                    p.Update(time);
                    
                    if (p.removeFlag)
                        projectiles.Remove().Release();

                    p = projectiles.Next();
                }
            }
        }

        public void AddZoneObserver(WorldObject obj)
        {
            zoneObservers.Add(obj);
        }

        public void RemoveZoneObserver(WorldObject obj)
        {
            zoneObservers.Remove(obj);

            if (obj == visualObserver)
                visualObserver = null;
        }

        public virtual void OnGUI()
        {
            foreach (KeyValuePair<ulong, ZoneGenerator> e in zoneGenerators)
                e.Value.OnGUI();
        }

        public virtual void Init()
        {

        }

        public virtual void FireWeapons(int obj, uint mask, int targetWorldObject, int targetComponent )
        {
            throw new Exception("not implemented");
        }

        public virtual void FireWeapons(int obj, uint mask, Vector3 targetPoint)
        {
            // WeaponProjectile
            WorldObject e = worldObjects[obj];

            if (e == null) return;

            foreach (WorldObjectComponentPropertyWeapon weap in e.indexWeapons)
            {
                if (!weap.activated) continue;

                if (((uint)(1 << (int)weap.weaponID) & mask) > 0)
                    projectiles.Add(new WeaponProjectile(weap, targetPoint));
            }
        }

        public virtual WorldObject CreateWorldObject(string rootComponent)
        {
            return CreateWorldObject(ArchModel.GetComponent(rootComponent));
        }

        public virtual WorldObject CreateWorldObject(ulong rootComponent)
        {
            return CreateWorldObject(ArchModel.GetComponent(rootComponent));
        }

        public virtual WorldObject CreateWorldObject(ArchWorldObjectComponent root)
        {
            return CreateWorldObject(root, worldObjects.freeSlot);
        }

        public virtual WorldObject CreateWorldObject(ArchWorldObjectComponent root, int id)
        {
            if (root == null)
                return null;

            WorldObject obj = new WorldObject(this, id, root);
            worldObjects.Insert(obj);
            return obj;
        }

        public virtual WorldObject GetWorldObject(int id)
        {
            return worldObjects[id];
        }

        public virtual void SetWorldObjectName(int id, string name)
        {
            WorldObject e = TryGetValidWorldObject(id);
            if (e == null) return;
            e.objectName = name;
        }

        public WorldObject TryGetValidWorldObject(int id)
        {
            WorldObject e = worldObjects[id];
            if (e == null) return null;
            if (!e.IsAlive()) return null;
            return e;
        }

        public virtual void SetWorldObjectPosition(int id, int component, Vector3 pos)
        {
            WorldObject e = TryGetValidWorldObject(id);
            if (e == null) return;

            if (e.indexComponents[component].hull == null) return;

            e.indexComponents[component].hull.SetPosition(pos);
        }

        public virtual void LoadSystem(ArchObjectSystem sys)
        {
            systemArch = sys;

           foreach (KeyValuePair<ulong, ArchObjectSystemSolar> e in sys.solars)
                CreateSolar(e.Value);

           foreach (KeyValuePair<ulong, ArchObjectSystemZone> e in sys.zones)
               CreateZone(e.Value);

           system = sys.id;
        }

        void CreateZone(ArchObjectSystemZone zone)
        {
            zoneGenerators.Add(zone.id, new ZoneGenerator(zone, this));
        }

        public virtual void CreateSolar(ArchObjectSystemSolar solar)
        {
            WorldObject obj = CreateWorldObject(ArchModel.GetComponent(solar.componentArchID));
            
            // setup root
            Transform tr = obj.GetRootHullRigidBody().transform;
            tr.localScale = Helpers.CopyVector3(solar.scale);
            tr.rotation = Quaternion.Euler(solar.rotation);
            tr.position = Helpers.CopyVector3(solar.position);

            obj.solarObjectData = solar;
            obj.type = WorldObjectType.Static;

            if (solar.solarObjectType == SolarType.Base)
            {
                obj.baseContents = new BaseContents(solar.baseContentsArch);
            }
        }

        public virtual void ClearWorld()
        {
            visualObserver = null;

            foreach (WorldObject e in worldObjects)
                worldObjects.Remove(e.id).Release();

            systemArch = null;

            foreach (KeyValuePair<ulong,ZoneGenerator> e in zoneGenerators)
                e.Value.Release();

            zoneGenerators.Clear();
        }

        public virtual void HandleProjectileHit(WeaponProjectile proj, WorldObjectComponent target)
        {
            RaycastHit hit = proj.rayCastInfo;
            proj.removeFlag = true;
            CreateEffect(proj.projectileDesc.projectile_hull_hit_effect, hit.point, proj.projectileDesc.projectile_hull_hit_effect_duration);
        }

        public virtual void CreateEffect(string prefab, Vector3 pos, float destroyTime)
        {
            GameObject obj = Helpers.LoadUnityObject(prefab);

            Helpers.SetLayerDeep(obj, layer);

            obj.transform.position = pos;

            UnityEngine.Object.Destroy(obj, destroyTime);
        }

        public virtual bool RemoveWorldObject(int id)
        {
            WorldObject e = GetWorldObject(id);
            if (e == null) return false;
            
            RemoveZoneObserver(e);
            e.Release();

            worldObjects.Remove(id);

            return true;
        }

        public virtual DockReply RequestDock(int objFrom, int objTo)
        {
            WorldObject robj = TryGetValidWorldObject(objFrom);
            WorldObject baseObj = TryGetValidWorldObject(objTo);

            if (robj == null) return DockReply.Error;
            if (baseObj == null) return DockReply.Error;
            if (!baseObj.isDockable) return DockReply.NoDock;

            return baseObj.RequestDock(robj);
        }

        public virtual bool Dock(int objFrom, int objTo)
        {
            return RemoveWorldObject(objFrom);
        }

        // net message handlers

        override protected void InitNetMessageHandlers()
        {
            base.InitNetMessageHandlers();

            msgHandlers[NetworkMessageType.TZSSetWorldObjectPosition] = this.HandleTZSSetWorldObjectPosition;
            msgHandlers[NetworkMessageType.TZSSetWorldObjectName] = this.HandleTZSSetWorldObjectName;
            msgHandlers[NetworkMessageType.TFireAllActiveWeapons] = this.HandleTFireAllActiveWeapons;
            msgHandlers[NetworkMessageType.TZCWorldObjectBasicUpdate] = this.HandleTZCWorldObjectBasicUpdate;
            msgHandlers[NetworkMessageType.TZSCreateWorldObject] = this.HandleTZSCreateWorldObject;
            msgHandlers[NetworkMessageType.TZSRemoveWorldObject] = this.HandleTZSRemoveWorldObject;
            msgHandlers[NetworkMessageType.TZSComponentHullData] = this.HandleTZSComponentHullData;
            msgHandlers[NetworkMessageType.TZSDestroyWorldObjectComponent] = this.HandleTZSDestroyWorldObjectComponent;
        }

        // add new handler here

        protected void HandleTZSDestroyWorldObjectComponent(NetworkMessage msg)
        {
            MsgTZSDestroyWorldObjectComponent m = msg as MsgTZSDestroyWorldObjectComponent;
            if (m == null) return;

            DestroyWorldObjectComponent(m.obj, m.component);
        }

        protected void HandleTZSComponentHullData(NetworkMessage msg)
        {
            MsgTZSComponentHullData m = msg as MsgTZSComponentHullData;
            if (m == null) return;

            WorldObject e = TryGetValidWorldObject(m.obj);

            if (e == null) return;

            e.indexComponents[m.component].hull.hullStructure.structurePoints = m.structure;
        }

        protected void HandleTZSRemoveWorldObject(NetworkMessage msg)
        {
            MsgTZSRemoveWorldObject m = msg as MsgTZSRemoveWorldObject;
            if (m == null) return;

            RemoveWorldObject(m.obj);
        }

        protected virtual void HandleTZSSetWorldObjectPosition(NetworkMessage msg)
        {
            MsgTZSSetWorldObjectPosition m = msg as MsgTZSSetWorldObjectPosition;
            if (m == null) return;

            SetWorldObjectPosition(m.obj, m.component, m.position);
        }

        protected virtual void HandleTZSSetWorldObjectName(NetworkMessage msg)
        {
            MsgTZSSetWorldObjectName m = msg as MsgTZSSetWorldObjectName;
            if (m == null) return;

            SetWorldObjectName(m.obj, m.name);
        }

        protected virtual void HandleTFireAllActiveWeapons(NetworkMessage msg)
        {
            MsgTFireAllActiveWeapons m = msg as MsgTFireAllActiveWeapons;
            if (m == null) return;

            FireWeapons(m.obj, m.weapons, m.targetPoint);
        }

        protected virtual void HandleTZCWorldObjectBasicUpdate(NetworkMessage msg)
        {
            MsgTZCWorldObjectBasicUpdate m = msg as MsgTZCWorldObjectBasicUpdate;
            if (m == null) return;

            WorldObject obj = GetWorldObject(m.obj);

            if (obj == null)
                return;

            if (obj.GetRootHullRigidBody() == null) return;

            Rigidbody body = obj.GetRootHullRigidBody().rigidBody;

            if (body == null) return;

            body.velocity = m.position - body.position;
            body.angularVelocity = Helpers.MakeAngularVelocity(body.rotation, m.rotation);
        }

        protected virtual void HandleTZSCreateWorldObject(NetworkMessage msg)
        {
            MsgTZSCreateWorldObject m = msg as MsgTZSCreateWorldObject;
            if (m == null) return;

            WorldObject e = CreateWorldObject(ArchModel.GetComponent(m.component), m.id);

            if (e == null) return;

            e.guid = m.guid;

            if (m.nickname.CompareTo("") != 0)
                e.objectName = m.nickname;

            e.simulationType = SimulationType.Server;
        }
    }
}
