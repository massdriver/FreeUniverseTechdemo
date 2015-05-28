using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net.Messages;

namespace FreeUniverse.Net
{
    public class DefaultNetworkMessageFactory : INetworkMessageFactory
    {
        public NetworkMessage Create(Lidgren.Network.NetIncomingMessage msg)
        {
            NetworkMessage networkMessage = null;

            NetworkMessageType msgType = NetworkMessage.PeekType(msg);

            switch (msgType)
            {
                case NetworkMessageType.TZSSetWorldObjectPosition:
                    networkMessage = new MsgTZSSetWorldObjectPosition();
                    break;

                case NetworkMessageType.TZSSetWorldObjectName:
                    networkMessage = new MsgTZSSetWorldObjectName();
                    break;

                case NetworkMessageType.TZSRemoveWorldObject:
                    networkMessage = new MsgTZSRemoveWorldObject();
                    break;

                case NetworkMessageType.TZSDestroyWorldObjectComponent:
                    networkMessage = new MsgTZSDestroyWorldObjectComponent();
                    break;

                case NetworkMessageType.TZSComponentHullData:
                    networkMessage = new MsgTZSComponentHullData();
                    break;
                   
                case NetworkMessageType.TZCClientProjectileHit:
                    networkMessage = new MsgTZCClientProjectileHit();
                    break;

                case NetworkMessageType.TFireAllActiveWeapons:
                    networkMessage = new MsgTFireAllActiveWeapons();
                    break;

                case NetworkMessageType.TZCWorldObjectBasicUpdate:
                    networkMessage = new MsgTZCWorldObjectBasicUpdate();
                    break;

                case NetworkMessageType.TZSControlWorldObjectByClient:
                    networkMessage = new MsgTZSControlWorldObjectByClient();
                    break;

                case NetworkMessageType.TZSCreateWorldObject:
                    networkMessage = new MsgTZSCreateWorldObject();
                    break;

                

                case NetworkMessageType.TZCConnect:
                    networkMessage = new MsgTZCConnect();
                    break;

                case NetworkMessageType.TZSConnectReply:
                    networkMessage = new MsgTZSConnectReply();
                    break;

                case NetworkMessageType.TZSPlayerConnectionStatus:
                    networkMessage = new MsgTZSPlayerConnectionStatus();
                    break;
            }

            if (networkMessage == null)
                return null;

            return networkMessage.Read(msg);
        }
    }


}
