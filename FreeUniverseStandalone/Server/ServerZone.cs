using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World;
using FreeUniverse.Net;
using UnityEngine;
using FreeUniverse.Core;
using FreeUniverse.Common;
using FreeUniverse.Arch;

namespace FreeUniverse.Server
{
    public class ServerZone : INetworkServerDelegate
    {
        Dictionary<ulong, WorldControllerServer> _worldControllers = new Dictionary<ulong, WorldControllerServer>();
        public Dictionary<ulong, WorldControllerServer> worldControllers { get { return _worldControllers; } }

        public ClientDesc[] players { get; private set; }

        NetworkServer _server;
        public NetworkServer server
        {
            get { return _server; }
        }

        IServerZoneClientHandler _zoneClientHandler;

        public ServerZone()
        {
            players = new ClientDesc[MAX_PLAYERS];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new ClientDesc();
            }
        }

        public const int TEST_PORT = 42599;
        public const int MAX_PLAYERS = ConstData.MAX_CLIENTS;

        public void Start()
        {
            if (_server != null) return;

            ConsoleLog.Write("Zone server starting");

            _worldControllers.Add(ConstData.TEST_SYSTEM, new WorldControllerServer(ConstData.LAYER_SERVER_WORLD, this));
            _worldControllers[ConstData.TEST_SYSTEM].LoadSystem(ArchModel.GetSystem(HashUtils.StringToUINT64("system_demo")));

            _server = new NetworkServer(MAX_PLAYERS, TEST_PORT);
            _server.netServerDelegate = this;

            _server.Start();

            _zoneClientHandler = new ServerZoneClientHandlerTest(this);

            ConsoleLog.Write("Zone server started");
        }

        ~ServerZone()
        {
            _server.Shutdown();
        }

        public void Update(float time)
        {
            if (_server != null) _server.Update();
            if (_zoneClientHandler != null) _zoneClientHandler.OnUpdate(time);
        }

        #region INetworkServerDelegate Members

        public void OnNetworkServerClientConnected(NetworkServer server, int client)
        {
            _zoneClientHandler.OnConnect(client);
        }

        public void OnNetworkServerClientDisconnected(NetworkServer server, int client)
        {
            _zoneClientHandler.OnDisconnect(client);
        }

        public void OnNetworkServerClientMessage(NetworkServer server, int client, NetworkMessage msg)
        {
            _zoneClientHandler.OnMessage(client, msg);
        }

        #endregion

        public void SendToAllPlayersExcept(int exceptClient, NetworkMessage msg, Lidgren.Network.NetDeliveryMethod type)
        {
            foreach (ClientDesc cl in players)
            {
                if (cl.client == -1)
                    continue;

                if (cl.client == exceptClient)
                    continue;

                server.Send(cl.client, msg, type);
            }
        }

        public void SendToAllPlayers(NetworkMessage msg, Lidgren.Network.NetDeliveryMethod type)
        {
            foreach (ClientDesc cl in players)
            {
                if (cl.client == -1)
                    continue;

                server.Send(cl.client, msg, type);
            }
        }
    }
}
