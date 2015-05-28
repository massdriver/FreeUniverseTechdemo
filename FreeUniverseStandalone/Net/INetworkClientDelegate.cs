using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace FreeUniverse.Net
{
    public interface INetworkClientDelegate
    {
        void OnNetworkClientConnect(NetworkClient client, int result);
        void OnNetworkClientDisconnect(NetworkClient client, int reason);
        void OnNetworkClientMessage(NetworkClient client, NetworkMessage message);
    }
}
