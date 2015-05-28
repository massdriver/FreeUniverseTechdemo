using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World;
using FreeUniverse.Net;
using FreeUniverse.Net.Messages;
using UnityEngine;
using FreeUniverse.Core;

namespace FreeUniverse.Client
{
    class ClientZoneServerHandlerTest : IClientZoneServerHandler
    {
        private ZoneClient zoneClient { get; set; }

        public ClientZoneServerHandlerTest(ZoneClient zc)
        {
            zoneClient = zc;
        }

        public void OnConnect()
        {
            MsgTZCConnect msg = new MsgTZCConnect();
            msg.playerNickname = zoneClient.playerNickname;
            msg.startTemplate = zoneClient.startTemplate;
            zoneClient.client.Send(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
        }

        public void OnDisconnect()
        {
            Debug.Log("ClientZoneServerHandlerTest: OnDisconnect");
        }

        public void OnMessage(NetworkMessage msg)
        {
            // Default to world controller
            switch (msg.type)
            {
                case NetworkMessageType.TZSConnectReply:
                    {    
                        MsgTZSConnectReply m = msg as MsgTZSConnectReply;
                       
                        zoneClient.clientid = m.client;
                        zoneClient.worldControllerClient.LoadSystem(ConstData.TEST_SYSTEM);

                        ConsoleLog.Write("ZC: TZSConnectReply id=" + m.client);
                    }
                    break;
                default:
                    zoneClient.worldControllerClient.ProcessNetworkMessage(msg);
                    break;
            }
        }


        public void OnUpdate(float time)
        {

        }

    }
}
