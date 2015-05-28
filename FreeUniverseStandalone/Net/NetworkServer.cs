using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using UnityEngine;

namespace FreeUniverse.Net
{
    public class NetworkServer
    {
        public INetworkMessageFactory messageFactory { get; set; }
        public INetworkServerDelegate netServerDelegate { get; set; }
        public int maxClients { get; private set; }
        private NetServer server { get; set; }
        private NetPeerConfiguration config { get; set; }
        private Stack<int> ids { get; set; }
        private Dictionary<int, ClientData> clients { get; set; }

        public NetworkServer(int maxClients, int port)
        {
            messageFactory = new DefaultNetworkMessageFactory();
            this.maxClients = maxClients;

            config = new NetPeerConfiguration("FreeUniverse");
            config.Port = port;
            config.ConnectionTimeout = 15.0f;
            config.MaximumConnections = maxClients;

            ids = new System.Collections.Generic.Stack<int>();

            for (int i = 0; i < maxClients; i++)
                ids.Push(i);

            clients = new System.Collections.Generic.Dictionary<int, ClientData>();

            server = new NetServer(config);
        }

        public void Start()
        {
            server.Start();
        }

        public void Shutdown()
        {
            server.Shutdown(null);
        }

        public void Send(int targetClient, NetworkMessage msg, NetDeliveryMethod deliveryType)
        {
            NetOutgoingMessage msgOut = server.CreateMessage();
            msg.Write(msgOut);

            if (targetClient == -1)
                server.SendToAll(msgOut, deliveryType);
            else
            {
                ClientData data;
                clients.TryGetValue(targetClient, out data);

                if (data == null) return;
                server.SendMessage(msgOut, clients[targetClient].sc, deliveryType);
            }
        }

        private class ClientData
        {
            public int id;
            public NetConnection sc;

            public ClientData(int id, NetConnection sc)
            {
                this.id = id;
                this.sc = sc;
            }
        }

        public string GetClientIP(int client)
        {
            return clients[client].sc.RemoteEndPoint.ToString();
        }

        public void Update()
        {
            NetIncomingMessage im;
            while ((im = server.ReadMessage()) != null)
            {
                // handle incoming message
                

                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                        {
                            int newClient = ids.Pop();

                            clients[newClient] = new ClientData(newClient, im.SenderConnection);
                            im.SenderConnection.Tag = clients[newClient];

                            if (netServerDelegate != null)
                                netServerDelegate.OnNetworkServerClientConnected(this, newClient);
                        }

                        if (status == NetConnectionStatus.Disconnected)
                        {
                            int clientID = ((ClientData)im.SenderConnection.Tag).id;

                            if (netServerDelegate != null)
                                netServerDelegate.OnNetworkServerClientDisconnected(this, clientID);

                            clients.Remove(clientID);
                            ids.Push(clientID);
                        }

                        break;
                    case NetIncomingMessageType.Data:
                        {
                            int clientID = ((ClientData)im.SenderConnection.Tag).id;
                            NetworkMessage flmsg = messageFactory.Create(im);

                            if ((netServerDelegate != null) && (flmsg != null))
                            {   
                                flmsg.clientID = clientID;
                                netServerDelegate.OnNetworkServerClientMessage(this, clientID, flmsg);
                            }

                        }
                        break;
                    default:
                        break;
                }
                server.Recycle(im);
            }

        }


    }
}
