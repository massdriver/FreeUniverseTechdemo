using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net;
using FreeUniverse.Net.Messages;
using FreeUniverse.World.Database;
using UnityEngine;
using FreeUniverse.Common.Database;
using FreeUniverse.Common;

namespace FreeUniverse.Server
{
    public class ServerLogin : NetworkMessageHandler, INetworkServerDelegate
    {
        public WorldDatabaseMaster wdb { get; set; }
        private NetworkServer server { get; set; }

        public ServerLogin()
        {
            InitNetMessageHandlers();
        }

        public void Start()
        {
            server = new NetworkServer(ConstData.MAX_CLIENTS, ConstData.LOGIN_SERVER_PORT);
            server.netServerDelegate = this;
            server.messageFactory = new NetworkMessageFactoryLoginServer();
            server.Start();

            Debug.Log("LS: login server started");
        }

        public void Shutdown()
        {
            server.Shutdown();
            server.netServerDelegate = null;
            server = null;
        }

        public void Update()
        {
            if (server != null)
                server.Update();
        }

        #region INetworkServerDelegate Members

        public void OnNetworkServerClientConnected(NetworkServer server, int client)
        {
            Debug.Log("LS: new connection client="+client);
        }

        public void OnNetworkServerClientDisconnected(NetworkServer server, int client)
        {
            Debug.Log("LS: connection lost client=" + client);
            wdb.OnPlayerClientDisconnect(client);
        }

        public void OnNetworkServerClientMessage(NetworkServer server, int client, NetworkMessage msg)
        {
            Debug.Log("LS: incoming msg from client=" + client);
            ProcessNetworkMessage(msg);
        }

        #endregion

        protected override void InitNetMessageHandlers()
        {
            base.InitNetMessageHandlers();

            // Account handlers
            msgHandlers[NetworkMessageType.CLAccountLoginRequest] = this.HandleCLAccountLoginRequest;
        }

        void HandleCLAccountLoginRequest(NetworkMessage m)
        {
            MsgCLAccountLoginRequest msg = m as MsgCLAccountLoginRequest;

            if (m == null) return;

            Account account = wdb.AccountLogin(msg.email, msg.password, m.clientID);

            MsgLSAccountAuthorizationReply msgout = new MsgLSAccountAuthorizationReply();

            if (account == null)
            {
                msgout.reply = MsgLSAccountAuthorizationReply.AUTHORIZATION_FAILED;
            }
            else
            {
                msgout.reply = m.clientID;
                msgout.account = account;
            }
            server.Send(m.clientID, msgout, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);

        }
    }
}
