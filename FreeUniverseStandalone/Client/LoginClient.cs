using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net;
using FreeUniverse.Net.Messages;
using UnityEngine;
using FreeUniverse.Common;
using FreeUniverse.Arch;


namespace FreeUniverse.Client
{
    public class LoginClient : NetworkMessageHandler, INetworkClientDelegate
    {
        public int clientid { get; private set; }
        private NetworkClient networkClient { get; set; }
        public ILoginClientDelegate loginClientDelegate { get; set; }
        private string email { get; set; }
        private string password { get; set; }
        public bool authorized { get; set; }
        public Account account { get; private set; }
        public List<ArchCharacterTemplate> templates { get; set; }

        public LoginClient()
        {
            authorized = false;
            templates = new List<ArchCharacterTemplate>();
            InitNetMessageHandlers();
        }

        const string LOGIN_SERVER_IP = "127.0.0.1";

        public void Start()
        {
            networkClient = new NetworkClient();
            networkClient.netClientDelegate = this;
            networkClient.Start();

            Debug.Log("Login Client: started");
        }

        public void Shutdown()
        {
            networkClient.Shutdown();
            networkClient.netClientDelegate = null;
            networkClient = null;
        }

        public void Update()
        {
            if( networkClient != null )
                networkClient.Update();
        }

        public void ConnectAndLogin(string email, string password)
        {
            if (networkClient == null)
                return;

            this.email = email;
            this.password = password;
            networkClient.Connect(LOGIN_SERVER_IP, ConstData.LOGIN_SERVER_PORT);
        }

        public void RequestAccountData()
        {

        }

        public void AbortConnection()
        {
            if( networkClient != null )
                networkClient.Disconnect();  
        }

        public void OnNetworkClientConnect(NetworkClient client, int result)
        {
            Debug.Log("LC: connected");

            if (!authorized)
            {
                Debug.Log("LC: sending authorization request to login server");

                MsgCLAccountLoginRequest msg = new MsgCLAccountLoginRequest();
                msg.email = this.email;
                msg.password = this.password;
                networkClient.Send(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void OnNetworkClientDisconnect(NetworkClient client, int reason)
        {
            Debug.Log("LC: disconnected");

            if (authorized && loginClientDelegate != null)
                loginClientDelegate.OnLoginClientConnectionLost(this);
        }

        public void OnNetworkClientMessage(NetworkClient client, NetworkMessage message)
        {
            ProcessNetworkMessage(message);
        }

        protected override void InitNetMessageHandlers()
        {
            base.InitNetMessageHandlers();

            msgHandlers[NetworkMessageType.LSAccountAuthorizationReply] = this.HandleLSAccountAuthorizationReply;
        }

        void HandleLSAccountAuthorizationReply(NetworkMessage msg)
        {
            MsgLSAccountAuthorizationReply m = msg as MsgLSAccountAuthorizationReply;

            if (m == null) return;

            if (m.reply == MsgLSAccountAuthorizationReply.AUTHORIZATION_FAILED)
            {
                Debug.Log("LC: failed to authorize");

                authorized = false;

                if (loginClientDelegate != null)
                    loginClientDelegate.OnLoginClientConnectionRefused(this);

                return;
            }

            Debug.Log("LC: authorized at login server");

            authorized = true;
            account = m.account;

            if (loginClientDelegate != null)
                loginClientDelegate.OnLoginClientConnectionAccept(this, m.account);
        }
    }
}
