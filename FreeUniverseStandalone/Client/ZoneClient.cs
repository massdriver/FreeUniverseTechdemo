using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World;
using FreeUniverse.Net;
using FreeUniverse.Net.Messages;


namespace FreeUniverse.Client
{
    public class ZoneClient : INetworkClientDelegate
    {
        public WorldControllerClient worldControllerClient { get; set; }
        public int clientid { get; set; }
        public NetworkClient client { get; private set; }
        private IClientZoneServerHandler zoneServerHandler { get; set; }
        public string playerNickname { get; set; }
        public ulong startTemplate { get; set; }

        public ZoneClient()
        {
            clientid = -1;
            zoneServerHandler = new ClientZoneServerHandlerTest(this);
        }

        public void Update(float time)
        {
            if (client != null) client.Update();
            if (zoneServerHandler != null) zoneServerHandler.OnUpdate(time);
        }

        public void ConnectToZoneServer(string ip, int port)
        {
            client = new NetworkClient();
            client.netClientDelegate = this;
            client.Start();
            client.Connect(ip, port);
        }

        public void DisconnectFromZoneServer()
        {
            if (client == null) return;

            client.Disconnect();
            client.Shutdown();
            client = null;
        }

        #region INetworkClientDelegate Members

        public void OnNetworkClientConnect(NetworkClient client, int result)
        {
            zoneServerHandler.OnConnect();
        }

        public void OnNetworkClientDisconnect(NetworkClient client, int reason)
        {
            zoneServerHandler.OnDisconnect();
        }

        public void OnNetworkClientMessage(NetworkClient client, NetworkMessage message)
        {
            zoneServerHandler.OnMessage(message);
        }

        #endregion

        
    }
}
