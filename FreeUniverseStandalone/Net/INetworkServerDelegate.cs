using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net
{
    public interface INetworkServerDelegate
    {
        void OnNetworkServerClientConnected(NetworkServer server, int client);
        void OnNetworkServerClientDisconnected(NetworkServer server, int client);
        void OnNetworkServerClientMessage(NetworkServer server, int client, NetworkMessage msg);
    }
}
