using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net;
using FreeUniverse.Core;

namespace FreeUniverse.Server
{
    public enum ChatChannelType
    {
        Default,
        Universe,
        Faction,
        Bounty,
        Trade,
        Private,
        Dev
    }

    public class ChatClientEntry
    {
        public int id { get; set; }
        public ulong guid { get; set; }
        public bool connected { get; set; }
    }

    public class ServerChat : INetworkServerDelegate, INetworkClientDelegate
    {
        public NetworkClient client { get; private set; }
        public NetworkServer server { get; private set; }
        public IndexArray<ChatClientEntry> clientEntries { get; private set; }

        public ServerChat()
        {

        }

        #region INetworkServerDelegate Members

        public void OnNetworkServerClientConnected(NetworkServer server, int client)
        {
            
        }

        public void OnNetworkServerClientDisconnected(NetworkServer server, int client)
        {
            
        }

        public void OnNetworkServerClientMessage(NetworkServer server, int client, NetworkMessage msg)
        {
            
        }

        #endregion

        #region INetworkClientDelegate Members

        public void OnNetworkClientConnect(NetworkClient client, int result)
        {
   
        }

        public void OnNetworkClientDisconnect(NetworkClient client, int reason)
        {

        }

        public void OnNetworkClientMessage(NetworkClient client, NetworkMessage message)
        {

        }

        #endregion
    }
}
