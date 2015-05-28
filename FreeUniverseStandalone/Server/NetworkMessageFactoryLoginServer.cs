using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net;
using FreeUniverse.Net.Messages;

namespace FreeUniverse.Server
{
    public class NetworkMessageFactoryLoginServer : INetworkMessageFactory
    {
        public NetworkMessage Create(Lidgren.Network.NetIncomingMessage msg)
        {
            NetworkMessage networkMessage = null;

            NetworkMessageType msgType = NetworkMessage.PeekType(msg);

            switch (msgType)
            {
                case NetworkMessageType.CLAccountLoginRequest:
                    networkMessage = new MsgCLAccountLoginRequest();
                    break;

                case NetworkMessageType.LSAccountAuthorizationReply:
                    networkMessage = new MsgLSAccountAuthorizationReply();
                    break;

                case NetworkMessageType.CLAccountCharacterAndSolarDataRequest:
                    networkMessage = new MsgCLAccountCharacterAndSolarDataRequest();
                    break;
            }

            if (networkMessage == null)
                return null;

            return networkMessage.Read(msg);
        }
    }
}
