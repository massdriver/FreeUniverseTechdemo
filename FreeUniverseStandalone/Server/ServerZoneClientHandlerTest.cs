using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net;
using FreeUniverse.World;
using FreeUniverse.Net.Messages;
using UnityEngine;
using FreeUniverse.Arch;
using FreeUniverse.Core;
using FreeUniverse.Common;

namespace FreeUniverse.Server
{
    public class ServerZoneClientHandlerTest : IServerZoneClientHandler
    {
        public ServerZone zoneServer { get; set; }

        public ServerZoneClientHandlerTest(ServerZone server)
        {
            zoneServer = server;
        }

        public void AddPlayer(int client)
        {
            ClientDesc cl = zoneServer.players[client];
            cl.Reset();
            cl.client = client;
        }

        #region IServerZoneClientHandler Members

        public void OnConnect(int client)
        {
            AddPlayer(client);
        }

        public void OnDisconnect(int client)
        {
            MsgTZSPlayerConnectionStatus status = new MsgTZSPlayerConnectionStatus();
            status.playerName = zoneServer.players[client].playerNickname;
            status.status = MsgTZSPlayerConnectionStatus.STATUS_DISCONNECTED;
            zoneServer.SendToAllPlayersExcept(client, status, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);

            zoneServer.players[client].world.RemoveWorldObject(zoneServer.players[client].worldObject);
            //MsgTZSRemoveWorldObject removeMsg = new MsgTZSRemoveWorldObject();
            //removeMsg.obj = zoneServer.players[client].worldObject;
            //zoneServer.SendToAllPlayersExcept(client, removeMsg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
            zoneServer.players[client].Reset();
        }

        public void OnMessage(int client, NetworkMessage msg)
        {
            switch (msg.type)
            {
                case NetworkMessageType.TZCWorldObjectBasicUpdate:
                    HandleMsgTZCWorldObjectBasicUpdate(client, msg as MsgTZCWorldObjectBasicUpdate);
                    break;
                case NetworkMessageType.TZCConnect:
                    HandleTZCConnect(client, msg as MsgTZCConnect);
                    break;
                case NetworkMessageType.TFireAllActiveWeapons:
                    HandleMsgTFireAllActiveWeapons(client, msg as MsgTFireAllActiveWeapons);
                    break;
                case NetworkMessageType.TZCClientProjectileHit:
                    HandleMsgTZCClientProjectileHit(client, msg as MsgTZCClientProjectileHit);
                    break;
            }
        }

        private void HandleMsgTZCClientProjectileHit(int client, MsgTZCClientProjectileHit msg)
        {
            if (!zoneServer.players[client].inWorld) return;

            zoneServer.players[client].world.ProcessNetworkMessage(msg);
        }

        void HandleMsgTFireAllActiveWeapons(int client, MsgTFireAllActiveWeapons msg)
        {
            if (!zoneServer.players[client].inWorld) return;

            zoneServer.players[client].world.ProcessNetworkMessage(msg);

            // resend
            zoneServer.players[client].world.SendToAllWorldPlayersExcept(client, msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
        }

        void HandleMsgTZCWorldObjectBasicUpdate(int client, MsgTZCWorldObjectBasicUpdate msg)
        {
            if (!zoneServer.players[client].inWorld) return;

            zoneServer.players[client].world.ProcessNetworkMessage(msg);

            // Resend to all clients here since msg doesnt contain client id
            zoneServer.players[client].world.SendToAllWorldPlayersExcept(client, msg, Lidgren.Network.NetDeliveryMethod.UnreliableSequenced);

        }

        void HandleTZCConnect(int client, MsgTZCConnect msg)
        {
            // send accept message to client
            MsgTZSConnectReply msg1 = new MsgTZSConnectReply();
            msg1.client = client;
            zoneServer.server.Send(client, msg1, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);

            zoneServer.players[client].playerNickname = msg.playerNickname;
            zoneServer.players[client].template = msg.startTemplate;
            zoneServer.players[client].system = ConstData.TEST_SYSTEM;
            zoneServer.players[client].world = zoneServer.worldControllers[ConstData.TEST_SYSTEM];

            // notify all other clients
            MsgTZSPlayerConnectionStatus status = new MsgTZSPlayerConnectionStatus();
            status.playerName = msg.playerNickname;
            status.status = MsgTZSPlayerConnectionStatus.STATUS_CONNECTED;
            zoneServer.SendToAllPlayersExcept(client, status, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);

            // create world object for player
            PlayerStartTemplate temp = PlayerStartTemplate.templates[msg.startTemplate];
            WorldObject obj = zoneServer.worldControllers[ConstData.TEST_SYSTEM].CreateWorldObject(
                ArchModel.GetComponent(temp.hull)
                );

            zoneServer.worldControllers[ConstData.TEST_SYSTEM].SetWorldObjectOwner(obj.id, client);
            zoneServer.worldControllers[ConstData.TEST_SYSTEM].ControlWorldObjectByClient(obj.id, client);
            zoneServer.worldControllers[ConstData.TEST_SYSTEM].SetWorldObjectName(obj.id, msg.playerNickname);
            

            Vector3 pos = temp.position + UnityEngine.Random.insideUnitSphere * ConstData.RANDOM_POSITION_FACTOR;
            zoneServer.worldControllers[ConstData.TEST_SYSTEM].SetWorldObjectPosition(obj.id, WorldObjectComponent.COMPONENT_ROOT, pos);

            // send new client world status
            {
                foreach (ClientDesc cl in zoneServer.players)
                {
                    if (cl.client == -1) continue;
                    if (cl.client == client) continue;

                    WorldObject e = zoneServer.worldControllers[ConstData.TEST_SYSTEM].GetWorldObject(cl.worldObject);

                    MsgTZSCreateWorldObject m = new MsgTZSCreateWorldObject();
                    m.component = PlayerStartTemplate.templates[cl.template].hull;
                    m.id = e.id;
                    m.guid = e.guid;
                    m.nickname = e.objectName;
                    zoneServer.server.Send(client, m, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
                }
            }
        }

        public void OnUpdate(float time)
        {
            foreach (KeyValuePair<ulong, WorldControllerServer> k in zoneServer.worldControllers)
                k.Value.Update(time);
        }

        #endregion

        

    }
}
