using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using FreeUniverse.Client;
using FreeUniverse.Net.Messages;
using FreeUniverse.World.Zone;
using UnityEngine;
using FreeUniverse.Core;
using FreeUniverse.Net;
using FreeUniverse.Common;

namespace FreeUniverse.World
{
    public class WorldControllerClient : WorldController, IWorldObjectDelegate
    {
        public WorldCameraSystem systemCamera { get; private set; }
        public WorldObjectControllerHuman humanController { get; private set; }
        public ZoneClient zoneClient { get; set; }
        public int clientWorldObject { get; set; }
        public bool inSpace { get; set; }

        private GameObject starsphere { get; set; }
        private GameObject background { get; set; }
        private float clientUpdateInterval { get; set; }

        public const float CLIENT_UPDATE_INTERVAL = 0.100f;

        public WorldControllerClient(int layer, int backGroundLayer)
            : base(layer)
        {
            inSpace = true;
            humanController = new WorldObjectControllerHuman();
            clientWorldObject = -1;
            visibleComponents = new List<WorldObjectComponent>();
        }

        private CharacterPersonality clientCharacterObj;

        public CharacterPersonality clientCharacter
        {
            get
            {
                return clientCharacterObj;
            }

            set
            {
                clientCharacterObj = value;
                OnCharacterSet();
            }
        }

        private void OnCharacterSet()
        {
            if (clientCharacter == null)
            {
                ClearWorld();
                return;
            }

            if (clientCharacter.location.staticBase != 0)
            {
                // Load base
                return;
            }
        }



        override public void Init()
        {
            systemCamera = new WorldCameraSystem(ConstData.LAYER_CLIENT_WORLD, ConstData.LAYER_STATIC_BACKGROUND);
        }

        public override void Update(float time)
        {
            base.Update(time);

            UpdateVisibleComponents();
            UpdateMouseSelectedComponent();

            if (humanController != null)
                humanController.Update(time);

            if (systemCamera != null)
                systemCamera.Update(time);

            clientUpdateInterval -= time;

            // send update to server
            if (humanController != null && humanController.controlledObject != null && clientUpdateInterval < 0.0f && humanController.controlledObject.IsAlive())
            {
                SendClientControllerWorldObjectUpdate();
                clientUpdateInterval = CLIENT_UPDATE_INTERVAL;
            }
        }

        public void SendClientControllerWorldObjectUpdate()
        {
            if (zoneClient != null && zoneClient.client != null && zoneClient.client.IsConnected())
            {
                Rigidbody body = humanController.controlledObject.GetRootHullRigidBody().rigidBody;
                MsgTZCWorldObjectBasicUpdate msg = new MsgTZCWorldObjectBasicUpdate();
                msg.obj = humanController.controlledObject.id;
                msg.position = body.position;
                msg.rotation = body.rotation;
                zoneClient.client.Send(msg, Lidgren.Network.NetDeliveryMethod.UnreliableSequenced);
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (humanController != null)
                humanController.OnGUI();

            if (systemCamera != null)
                systemCamera.OnGUI();
        }

        public void ControlWorldObjectByHuman(int id)
        {
            WorldObject obj = TryGetValidWorldObject(id);

            if (obj == null)
                clientWorldObject = -1;
            else
                clientWorldObject = id;

            humanController.SetWorldObject(obj);
        }

        public override bool Dock(int objFrom, int objTo)
        {
            if (clientWorldObject == objFrom)
            {
                RemoveZoneObserver(worldObjects[clientWorldObject]);
                DisableControlWorldObjectByHuman();
            }

            return base.Dock(objFrom, objTo);
        }

        public void DisableControlWorldObjectByHuman()
        {
            humanController.SetWorldObject(null);
            clientWorldObject = -1;
        }

        public void FollowThirdPersonCamera(int id)
        {
            WorldObject e = TryGetValidWorldObject(id);

            if (e == null) return;

            systemCamera.FollowThirdPerson(e);
            visualObserver = e;
        }

        public void UnFollowThirdPersonCamera()
        {
            if (_visualObserver != null)
                zoneObservers.Remove(_visualObserver);

            systemCamera.DisableCameraBehaviour();
        }

        public override void FireWeapons(int obj, uint mask, Vector3 targetPoint)
        {
            // if its client fire, then send to zone server
            if ((obj == clientWorldObject) && IsConnectedToServer())
            {
                MsgTFireAllActiveWeapons msg = new MsgTFireAllActiveWeapons();
                msg.obj = obj;
                msg.weapons = mask;
                msg.targetPoint = targetPoint;
                zoneClient.client.Send(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
            }

            base.FireWeapons(obj, mask, targetPoint);
        }

        public void LoadSystem(ulong id)
        {
            LoadSystem(ArchModel.GetSystem(id));
        }

        public void LoadSystem(string name)
        {
            LoadSystem(ArchModel.GetSystem(HashUtils.StringToUINT64(name)));
        }

        public bool IsConnectedToServer()
        {
            return zoneClient != null && zoneClient.client != null && zoneClient.client.IsConnected();
        }

        public override void HandleProjectileHit(WeaponProjectile proj, WorldObjectComponent target)
        {
            if (target != null)
            {
                if (!IsConnectedToServer())
                {
                    target.InflictDamageToHull(proj.projectileDesc);
                }
                else if ( (clientWorldObject != -1) && (proj.parentGUID == worldObjects[clientWorldObject].guid))
                {
                    // Send hit event to server, client projectile localy hit some non-static object
                    MsgTZCClientProjectileHit msg = new MsgTZCClientProjectileHit();
                    msg.obj = target.parentWorldObject.id;
                    msg.component = target.id;
                    msg.arch = proj.componentArch;
                    msg.archIndex = proj.componentArchIndex;
                    zoneClient.client.Send(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
                }
            }

            base.HandleProjectileHit(proj, target);
        }

        public override void LoadSystem(FreeUniverse.Arch.ArchObjectSystem system)
        {
            if (null == system)
                return;

            ClearWorld();

            base.LoadSystem(system);

            // Additionally load static background stars + environment
            LoadStaticBackground(system);

            // Apply system back color to camera
            //_systemCamera.ApplySystemParameters(system);

            // Set shader light positions for atmosphere effect
            {
                Shader.SetGlobalVector("_StarLightPosition0", new Vector4(0.0f, 0.0f, 150000.0f, 1.0f));
                Shader.SetGlobalVector("_StarLightPosition1", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
                Shader.SetGlobalVector("_StarLightPosition2", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
                Shader.SetGlobalVector("_StarLightPosition3", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            }
        }

        public override void ClearWorld()
        {
            base.ClearWorld();

            if (null != starsphere)
            {
                UnityEngine.Object.Destroy(starsphere);
                starsphere = null;
            }

            if (null != background)
            {
                UnityEngine.Object.Destroy(background);
                background = null;
            }
        }

        public override void DestroyWorldObjectComponent(int obj, int component)
        {
            if (IsConnectedToServer())
                return;

            base.DestroyWorldObjectComponent(obj, component);
        }

        protected override void OnVisualObserverChanged()
        {
            if (visualObserver == null)
            {
                systemCamera.DisableCameraBehaviour();
            }

            base.OnVisualObserverChanged();
        }

        void LoadStaticBackground(FreeUniverse.Arch.ArchObjectSystem system)
        {
            if (null != system.stars)
            {
                starsphere = (UnityEngine.GameObject)UnityEngine.Object.Instantiate((UnityEngine.GameObject)Resources.Load(system.stars));

                if (null != starsphere)
                {
                    starsphere.layer = ConstData.LAYER_STATIC_BACKGROUND;

                    for (int i = 0; i < starsphere.transform.childCount; i++)
                    {
                        starsphere.transform.GetChild(i).gameObject.layer = starsphere.layer;
                    }
                }
            }

            if (null != system.background)
            {
                background = (UnityEngine.GameObject)UnityEngine.Object.Instantiate(Resources.Load(system.background));

                if (null != background)
                {
                    background.layer = ConstData.LAYER_STATIC_BACKGROUND;

                    for (int i = 0; i < background.transform.childCount; i++)
                    {
                        background.transform.GetChild(i).gameObject.layer = background.layer;
                    }
                }
            }
        }

        #region IWorldObjectDelegate Members

        public void OnWorldObjectReleased(WorldObject obj)
        {
            if (obj.id == clientWorldObject)
            {
                DisableControlWorldObjectByHuman();
                UnFollowThirdPersonCamera();
                clientWorldObject = -1;
            }
        }

        #endregion

        public WorldObjectComponent mouseSelectedComponent { get; private set; }

        void UpdateMouseSelectedComponent()
        {
            mouseSelectedComponent = null;
            if (systemCamera == null) return;
            if (systemCamera.camera == null) return;

            RaycastHit hit;
            Ray ray = systemCamera.camera.ViewportPointToRay(ApplicationModel.inputController.viewportMousePosition);
            
            if (!Physics.Raycast(ray, out hit)) return;

            RayCastProvider p = hit.collider.gameObject.GetComponent<RayCastProvider>();

            if (p == null) return;
            if (p.worldRidigBody == null) return;
            if (p.worldRidigBody.parentHullProperty == null) return;

            mouseSelectedComponent = p.worldRidigBody.parentHullProperty.parentComponent;
        }

        public List<WorldObjectComponent> visibleComponents { get; private set; }

        public void UpdateVisibleComponents()
        {
            visibleComponents.Clear();

            foreach (WorldObject k in worldObjects)
            {
                if (!k.IsAlive()) continue;

                Vector3 pos = k.rootHullPosition;
                Vector3 proj = systemCamera.camera.WorldToViewportPoint(pos);

                if (proj.x >= 0.0f && proj.x <= 1.0f &&
                    proj.y >= 0.0f && proj.y <= 1.0f && proj.z > 0.0f)
                {
                    visibleComponents.Add(k.root);
                }
            }
        }

        protected override void InitNetMessageHandlers()
        {
            base.InitNetMessageHandlers();

            msgHandlers[NetworkMessageType.TZSPlayerConnectionStatus] = this.HandleTZSPlayerConnectionStatus;
            msgHandlers[NetworkMessageType.TZSControlWorldObjectByClient] = this.HandleTZSControlWorldObjectByClient;
        }

        protected void HandleTZSPlayerConnectionStatus(NetworkMessage msg)
        {
            MsgTZSPlayerConnectionStatus m = msg as MsgTZSPlayerConnectionStatus;
            if (m == null) return;
        }

        protected void HandleTZSControlWorldObjectByClient(NetworkMessage msg)
        {
            MsgTZSControlWorldObjectByClient m = msg as MsgTZSControlWorldObjectByClient;
            if (m == null) return;

            if (m.status == MsgTZSControlWorldObjectByClient.STATUS_ENABLED)
            {
                ControlWorldObjectByHuman(m.obj);
                FollowThirdPersonCamera(m.obj);
            }
            else if (m.status == MsgTZSControlWorldObjectByClient.STATUS_DISABLED)
            {
                DisableControlWorldObjectByHuman();
                UnFollowThirdPersonCamera();
            }
        }
    }
}
