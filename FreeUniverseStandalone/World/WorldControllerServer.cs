using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Server;
using FreeUniverse.Net;
using FreeUniverse.Net.Messages;
using FreeUniverse.Arch;
using FreeUniverse.Core;
using UnityEngine;
using FreeUniverse.Common;

namespace FreeUniverse.World
{
    public class WorldControllerServer : WorldController
    {
        public ServerZone zoneServer { get; set; }

        public WorldControllerServer(int layerid, ServerZone server) : base(layerid)
        {
            zoneServer = server;
        }

        public void SetWorldObjectOwner(int obj, int client)
        {
            zoneServer.players[client].worldObject = obj;
            zoneServer.players[client].system = system;
        }

        public override WorldObject CreateWorldObject(FreeUniverse.Arch.ArchWorldObjectComponent root)
        {
            WorldObject obj = base.CreateWorldObject(root);

            // send message
            {
                MsgTZSCreateWorldObject msg = new MsgTZSCreateWorldObject();
                msg.component = root.id;
                msg.id = obj.id;
                msg.guid = obj.guid;
                SendToAllWorldPlayers(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
            }

            return obj;
        }

        public void ControlWorldObjectByClient(int obj, int client )
        {
            WorldObject wobject = TryGetValidWorldObject(obj);

            if (wobject == null) return;
            if (!CheckClientSystem(client)) return;

            // Send update
            {
                MsgTZSControlWorldObjectByClient msg = new MsgTZSControlWorldObjectByClient();
                msg.obj = obj;
                msg.client = client;
                msg.status = MsgTZSControlWorldObjectByClient.STATUS_ENABLED;
                zoneServer.server.Send(client, msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
            }

            zoneServer.players[client].worldObject = obj;
        }

        bool CheckClientSystem(int client)
        {
            if (zoneServer.players[client].system == system)
                return true;

            return false;
        }

        void HandleProjectileHit(int obj, int component, ulong arch)
        {
            ArchWorldObjectComponent comp = ArchModel.GetComponent(arch);

            if (comp == null)return;

            WorldObject e = TryGetValidWorldObject(obj);
            if (e == null) return;

            if (worldObjects[obj].indexComponents[component].hull == null)return;

            float newval = worldObjects[obj].indexComponents[component].InflictDamageToHull(comp.weapon.projectileDesc);

            MsgTZSComponentHullData msg = new MsgTZSComponentHullData();
            msg.obj = obj;
            msg.component = component;
            msg.structure = newval;
            SendToAllWorldPlayers(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
        }

        public override void SetWorldObjectPosition(int id, int component, UnityEngine.Vector3 pos)
        {
            base.SetWorldObjectPosition(id, component, pos);

            MsgTZSSetWorldObjectPosition msg = new MsgTZSSetWorldObjectPosition();
            msg.obj = id;
            msg.component = component;
            msg.position = pos;
            SendToAllWorldPlayers(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
        }

        public override void DestroyWorldObjectComponent(int obj, int component)
        {
            // send msg
            MsgTZSDestroyWorldObjectComponent msg = new MsgTZSDestroyWorldObjectComponent();
            msg.obj = obj;
            msg.component = component;
            SendToAllWorldPlayers(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);

            int deadClient = -1;
            {
                
                foreach (ClientDesc cl in zoneServer.players)
                {
                    if (obj == cl.worldObject)
                    {
                        cl.worldObject = -1;
                        deadClient = cl.client;
                    }
                }
            }

            base.DestroyWorldObjectComponent(obj, component);

            // respawn
            
            if( deadClient != -1 )
            {
                WorldObject e = CreateWorldObject(
ArchModel.GetComponent(PlayerStartTemplate.templates[zoneServer.players[deadClient].template].hull)
);
                SetWorldObjectOwner(e.id, deadClient);
                ControlWorldObjectByClient(e.id, deadClient);
                SetWorldObjectName(e.id, zoneServer.players[deadClient].playerNickname);

                Vector3 pos = PlayerStartTemplate.templates[zoneServer.players[deadClient].template].position + UnityEngine.Random.insideUnitSphere * ConstData.RANDOM_POSITION_FACTOR;
                SetWorldObjectPosition(e.id, WorldObjectComponent.COMPONENT_ROOT, pos);
            }
             
        }

        public override void SetWorldObjectName(int id, string name)
        {
            base.SetWorldObjectName(id, name);

            MsgTZSSetWorldObjectName msg = new MsgTZSSetWorldObjectName();
            msg.obj = id;
            msg.name = name;
            SendToAllWorldPlayers(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
        }

 

        // misc

        public void SendToAllWorldPlayersExcept(int exceptClient, NetworkMessage msg, Lidgren.Network.NetDeliveryMethod type)
        {
            foreach (ClientDesc cl in zoneServer.players)
            {
                if (cl.client == -1)
                    continue;

                if (cl.client == exceptClient)
                    continue;

                if (cl.system != system)
                    continue;

                zoneServer.server.Send(cl.client, msg, type);
            }
        }

        public void SendToAllWorldPlayers(NetworkMessage msg, Lidgren.Network.NetDeliveryMethod type)
        {
            foreach (ClientDesc cl in zoneServer.players)
            {
                if (cl.client == -1)
                    continue;

                if (cl.system != system)
                    continue;

                zoneServer.server.Send(cl.client, msg, type);
            }
        }

        public override bool RemoveWorldObject(int id)
        {
            if (TryGetValidWorldObject(id) == null) return false;

            MsgTZSRemoveWorldObject msg = new MsgTZSRemoveWorldObject();
            msg.obj = id;
            SendToAllWorldPlayers(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);

            return base.RemoveWorldObject(id);
        }

        protected override void InitNetMessageHandlers()
        {
            base.InitNetMessageHandlers();

            msgHandlers[NetworkMessageType.TZCClientProjectileHit] = this.HandleMsgTZCClientProjectileHit;
        }

        void HandleMsgTZCClientProjectileHit(NetworkMessage msg)
        {
            MsgTZCClientProjectileHit m = msg as MsgTZCClientProjectileHit;
            if (m == null) return;

            HandleProjectileHit(m.obj, m.component, m.arch);
        }
    }
}
